# BNJMO-Framework
Plugin framework for Unity.
All rights reserved to BNJMO Studios.


### How to add this plugin as a submodule to another Unity repository?
git submodule add https://github.com/BNJMO/BNJMO-Framework.git Assets/Plugins/BNJMO-Framework/

### How to checkout the right commit of the submodule in a Unity repository?
git submodule update --init --recursive --force

### How to update the submodule from a Unity repository?
cd Assets/Plugins/BNJMO-Framework
git add/commit/push/checkout...


### How to configure project's namespace for BBehaviour creation context?
Under a **Resources** folder, create a .txt file called **ProjectConfig**:

[App]

NameSpace = ProjectName
