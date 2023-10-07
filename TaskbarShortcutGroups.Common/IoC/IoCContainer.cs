using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TaskbarShortcutGroups.Common.Extensions;

namespace TaskbarShortcutGroups.Common.IoC;

/// <summary>
/// Represents IoC container which allows injecting dependencies into newly created objects.
/// </summary>
/// <remarks>
/// This definitely is not the best implementation (it rather is one of the worst ones out there xD) but it was created
/// for learning purposes. If need arises I will switch to some framework.
/// </remarks>
public class IoCContainer : IDisposable
{
    private readonly IDictionary<Type, object> singletonScope;
    private readonly IDictionary<Type, TypeRegistration> typeRegistrations;

    private IoCContainer()
    {
        singletonScope = new Dictionary<Type, object>();
        typeRegistrations = new Dictionary<Type, TypeRegistration>();
    }

    public static IoCContainer Container { get; } = new();

    public void Dispose()
        => singletonScope.Values
            .Where(i => i is IDisposable)
            .Cast<IDisposable>()
            .ForEach(i => i.Dispose());

    public IoCContainer RegisterType<TBaseType, TImplementation>(LifeTime lifeTime) =>
        RegisterType(typeof(TBaseType), typeof(TImplementation), lifeTime);

    public IoCContainer RegisterType(Type baseType, Type implementationType, LifeTime lifeTime)
    {
        if (!typeRegistrations.TryAdd(baseType, new TypeRegistration(baseType, implementationType, lifeTime)))
            throw new ArgumentException("Cannot register same type twice");

        return this;
    }

    public IoCContainer RegisterSingleton<TInterface, TImplementation>()
        => RegisterType<TInterface, TImplementation>(LifeTime.Singleton);

    public IoCContainer RegisterTransient<TInterface, TImplementation>()
        => RegisterType<TInterface, TImplementation>(LifeTime.Transient);

    public IoCContainer RegisterFactory<TImplementation>() =>
        RegisterTransient<IFactory<TImplementation>, Factory<TImplementation>>();

    public IoCContainer RegisterFactory<TInterface, TImplementation>() =>
        RegisterTransient<IFactory<TInterface>, Factory<TImplementation>>();

    public IoCContainer Register<T>(T implementation)
    {
        ExceptionExtensions.ThrowIfNull(implementation);
        AddSingletonToScope(implementation);
        return RegisterType(typeof(T), implementation.GetType(), LifeTime.Singleton);
    }

    public T Resolve<T>() => (T)Resolve(typeof(T));

    public T Resolve<T>(params object[] additionalParameters) => (T)Resolve(typeof(T), additionalParameters);

    public object Resolve(Type requestedType)
    {
        if (!typeRegistrations.TryGetValue(requestedType, out var typeRegistration))
            throw new InvalidOperationException($"Cannot construct non-registered instance of type '{requestedType}'");
        if (singletonScope.TryGetValue(typeRegistration.ImplementationType, out var instance))
            return instance;
        if (typeRegistration.LifeTime is not LifeTime.Singleton)
            return typeRegistration.GetInstance();
        var newInstance = typeRegistration.GetInstance();
        AddSingletonToScope(newInstance);
        return newInstance;
    }

    private void AddSingletonToScope<T>([NotNull] T instance)
    {
        if (!singletonScope.TryAdd(instance!.GetType(), instance))
            throw new ArgumentException(
                $"Cannot register same singleton instance of type `{instance.GetType()}` twice");
    }

    public object Resolve(Type requestedType, params object[] additionalParameters)
    {
        if (singletonScope.TryGetValue(requestedType, out var instance))
            return instance;
        if (!typeRegistrations.TryGetValue(requestedType, out var typeRegistration))
            throw new InvalidOperationException($"Cannot construct non-registered instance of type '{requestedType}'");
        return typeRegistration.GetInstance(additionalParameters);
    }

    private class Factory<T> : IFactory<T>
    {
        private readonly TypeRegistration typeRegistration = new(typeof(T), LifeTime.Transient);

        public T Construct() => (T)typeRegistration.GetInstance();

        public T Construct(params object[] parameters) => (T)typeRegistration.GetInstance(parameters);
    }

    private class TypeRegistration
    {
        private readonly ObjectFactory[] objectFactories;

        public TypeRegistration(Type baseType, Type implementationType, LifeTime lifeTime)
        {
            if (!baseType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"'{implementationType.Name}' is not of type '{baseType}'");
            ImplementationType = implementationType;
            LifeTime = lifeTime;
            var a = implementationType.GetConstructors();
            var b = a.First();
            var c = b.GetParameters();
            objectFactories = ImplementationType.GetConstructors().Select(c => new ObjectFactory(c)).ToArray();
            if (objectFactories.Length == 0)
                throw new ArgumentException(
                    $"Implementation type '{ImplementationType.Name}' contains no public constructors.");
        }

        public TypeRegistration(Type implementationType, LifeTime lifeTime)
        {
            ImplementationType = implementationType;
            LifeTime = lifeTime;
            objectFactories = ImplementationType.GetConstructors().Select(c => new ObjectFactory(c)).ToArray();
            if (objectFactories.Length == 0)
                throw new ArgumentException(
                    $"Implementation type '{ImplementationType.Name}' contains no constructors.");
        }

        public Type ImplementationType { get; }
        public LifeTime LifeTime { get; }

        public object GetInstance()
        {
            foreach (var factory in objectFactories)
                try
                {
                    return factory.Construct();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            throw new InvalidOperationException($"Failed to construct instance of type '{ImplementationType}'");
        }

        public object GetInstance(object[] additionalParameters)
        {
            var providedParameterTypes = additionalParameters.Select(p => p.GetType());
            var fittingFactories = objectFactories.Where(f => f.UsesParameterTypes(providedParameterTypes)).ToArray();
            if (fittingFactories.Length == 0)
                throw new ArgumentException("There is no constructor fitting to the provided parameters");
            foreach (var factory in fittingFactories)
                try
                {
                    return factory.Construct(additionalParameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            throw new InvalidOperationException($"Failed to construct instance of type '{ImplementationType}'");
        }

        private class ObjectFactory
        {
            private readonly ConstructorInfo constructorInfo;
            private readonly Type[] constructorParameterTypes;

            public ObjectFactory(ConstructorInfo constructorInfo)
            {
                this.constructorInfo = constructorInfo;
                constructorParameterTypes = constructorInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            public object Construct() =>
                constructorInfo.Invoke(constructorParameterTypes
                    .Select(t => Container.Resolve(t))
                    .ToArray());

            public object Construct(object[] providedParameters) =>
                constructorInfo.Invoke(constructorParameterTypes
                    .Select(t => providedParameters.FirstOrDefault(t.IsInstanceOfType) ?? Container.Resolve(t))
                    .ToArray());

            public bool UsesParameterTypes(IEnumerable<Type> typesToVerify) =>
                typesToVerify.All(constructorParameterTypes.Contains);
        }
    }
}