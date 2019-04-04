# Java Examples for NEO

This repository contains resources and quick starts to help Java developers get started on the NEO blockchain.

## Setup
You will need to setup a few things before getting started with these examples.

**Required**
 * [neo-devpack-java](https://github.com/neo-project/neo-devpack-java) – Java Native Interfaces (JNI) for Neo's methods
 * [neo-compiler](https://github.com/neo-project/neo-compiler) – Converter from Java bytecode to Neo bytecode


 **Advised**
 * [neo-privatenet](https://hub.docker.com/r/cityofzion/neo-privatenet/) – Docker image for local testing 
 * [neo-python](https://github.com/CityOfZion/neo-python) – CLI for NEO


For instructions on the setup process, see the [NEO docs](http://docs.neo.org)

## Development Notes

The Java source code that is written is converted to an avm file (Neo's Virtual Machine bytecode) by the neoj neo-compiler. There are a few limitations with this compiler in its current state that are important to mention.

1. Method calls while instantiating a field variable will result in runtime errors. Example:
Do not do the following
```java
public static final byte[] OWNER =
              Helper.asByteArray"AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y";
```

2. Package names cannot start with **org.neo** as they will be ignored by the neoj neo-compiler.
```csharp
 var bskip = cc.classfile.Name.IndexOf("org.neo.") == 0 || cc.classfile.Name.IndexOf("src.org.neo.") == 0;
```
See [neo-compiler's JAVAModule](https://github.com/neo-project/neo-compiler/blob/92533c40058cdda9be67e94a0e13712f16017b8c/neoj/JVM/JAVAModule.cs#L55)
