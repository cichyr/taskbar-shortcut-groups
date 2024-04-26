# TaskbarShortcutGroups

## About

This app enables you to group multiple shortcuts into one and select them from a single taskbar icon.

## Should I use this?

TL;DR: you'll be just fine with [Taskbar Groups](https://github.com/tjackenpacken/taskbar-groups), at least until v1.0
release (or maybe sooner).

It depends.
This software is just a tool that I've quickly put together and have been using because other had some shortcomings and,
as such, comes with few of its own.
It is not tested, performance is not the best, code is not the best one out there, there's little to no user guidance.
However, it does the job, and when opening the shortcut group, you'll see full names of the contained shortcuts, not
only their icons.
For me, it was a huge issue with Taskbar Groups.

## Installation instructions

Requirements: 64-bit Windows. There is no Linux or MacOS support.
From releases download .zip file and unzip it at any location.

## Updating instructions

Remove everything except the "Config" folder and install the new version in the same directory.

## How to use the app

### TL;DR

1. Press "Add new shortcut group" to create a new group.
2. Set the name and icon for the group.
3. Add shortcuts by pressing "Add new shortcut".
4. Press "Save" and then "Back".
5. Press "Open shortcuts location".
6. Double click created shortcut
7. Right-click on icon that shows up on the taskbar and press "Pin to taskbar". 

### Group list view

After startup, you'll be presented with a list of shortcut groups.
Here you can press any of the groups to edit them or "Add new shortcut group" to create a new group.
You can also navigate to "About" view or open directory containing shortcuts to all visible groups using buttons on the bottom.
Also from directory you can right-click the shortcut to pin it to taskbar.

### Group editor view

Inside the group editor, you can fill the name, fill the group icon, and add or remove shortcuts.

- Name should be unique.
- Icon can be any PNG or JPEG icon or prepared ICO icon file.
- Shortcuts can be added via separate button or removed using button with trash bin icon.

On the bottom there are "Save" and "Back" buttons.
Only pressing the "Save" button saves state, and it doesn't navigate back to list view.
Right now there's no notification that saving has succeeded.

### About view

Here is information about the app version and used third-party components.
Also, if there's a newer version available on GitHub, there will be information about it.

### Shortcuts view

This view is visible when launching the app using one of the shortcuts to group.
Here you can see all icons and names to the shortcuts.
After pressing on the shortcut, it will be opened using the default application associated with the target file
extension.

## Planned features

Here are some features that I plan to add in the near-ish future:

- Reordering of shortcuts within the group
- Improved startup performance
- Validations
- Specifying the startup app per shortcut
- Automatic installer
- Customizing

Feel free to request your own!