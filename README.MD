# Schema Browser
This tool aims to provide a viable solution for anyone who wants to use a graphical user 
interface when browsing XML schema files (*.xsd)

## How to build
First, you need to setup a few prerequisites to be able to build this tool. The most easy
way is to just run the script `tools\setup.cmd`. But you may also run the commands 
individually, if you aren't on windows, for example:

    npm install --global --production windows-build-tools
    npm install --global node-gyp
    
This enables you to import the Java integration module which connects Java with node.js.

Once the prerequisites are in place, you can run:

    call tools\env.cmd
    mvn package
    
The first line is required once per shell. It sets the environment variables so that MSBuild
and Python can be located by the Java integration and node-gyp. See more details here:
https://github.com/joeferner/node-java

## How to run
You can start the program with node.js:

    cd target
    node index.js
    
    