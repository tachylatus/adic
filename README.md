# ![Adic](https://cloud.githubusercontent.com/assets/5340818/6597415/4b95cb42-c7db-11e4-863f-9a284bfab310.png)

***Another* Dependency Injection Container for Unity 3D and beyond**

[![Unity Asset Store](https://cloud.githubusercontent.com/assets/5340818/6855739/9e14c9e0-d3d9-11e4-9171-594941ed056f.png)](https://www.assetstore.unity3d.com/#!/content/32157)

## Contents

1. <a href="#introduction">Introduction</a>
2. <a href="#features">Features</a>
3. <a href="#concepts">Concepts</a>
	1. <a href="#what-is">What is a dependency injector container?
	2. <a href="#structure">Structure
	3. <a href="#types-of-bindings">Types of bindings
	4. <a href="#namespace-conventions">Namespace conventions
	5. <a href="#chaining">Chaining</a>
4. <a href="#quick-start">Quick start</a>
5. <a href="#api">API</a>
	1. <a href="#bindings">Bindings</a>
	2. <a href="#constructor-injection">Constructor injection</a>
	3. <a href="#member-injection">Member injection</a>
	4. <a href="#multiple-constructors">Multiple constructors</a>
	5. <a href="#multiple-injection">Multiple injection</a>
	6. <a href="#monobehaviour-injection">MonoBehaviour injection</a>
	7. <a href="#conditions">Conditions</a>
	8. <a href="#update">Update</a>
	9. <a href="#dispose">Dispose</a>
	10. <a href="#manual-type-resolution">Manual type resolution</a>
	11. <a href="#factories">Factories</a>
	12. <a href="#bindings-setup">Bindings setup</a>
	13. <a href="#using-commands">Using commands</a>
6. <a href="#order-of-events">Order of events
7. <a href="#performance">Performance
8. <a href="#container-extensions">Extensions</a>
	1. <a href="#available-extensions">Available extensions</a>
		1. <a href="#extension-bindings-printer">Bindings Printer</a>
		1. <a href="#extension-bindings-setup">Bindings Setup</a>
		2. <a href="#extension-commander">Commander</a>
		3. <a href="#extension-context-root">Context Root</a>
		4. <a href="#extension-event-caller">Event Caller</a>
		5. <a href="#extension-mono-injection">Mono Injection</a>
		6. <a href="#extension-unity-binding">Unity Binding</a>
	2. <a href="#creating-extensions">Creating extensions</a>
	3. <a href="#container-events">Container events</a>
		1. <a href="#binder-events">Binder events</a>
		2. <a href="#injector-events">Injector events</a>
9. <a href="#general-notes">General notes</a>
10. <a href="#examples">Examples</a>
11. <a href="#changelog">Changelog</a>
12. <a href="#support">Support</a>
13. <a href="#license">License</a>

## <a id="introduction"></a>Introduction

*Adic* is a lightweight dependency injection container for Unity 3D and any C# (or .Net) project.

Based on the proof of concept container from [Sebastiano Mandalà](http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/) and studies of [StrangeIoC](http://strangeioc.github.io/strangeioc/), the intention of the project is to create a dependency injection container that is simple to use and extend, having on its roots the simplicity of the work of Mandalà and the extensibility of StrangeIoC, also borrowing some ideas from the classic [Unity Application Block](https://unity.codeplex.com/).

The project is compatible with Unity 3D 5 and 4 and possibly 3 (not tested) and should work on all available platforms (tested on Windows/Mac/Linux, Android, iOS and Web Player).

Also available in the [Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/content/32157).

## <a id="features"></a>Features

* Bind types, singleton instances, factories, game objects and prefabs.
* Instance resolution by type, identifier and complex conditions.
* Injection on constructor, fields and properties.
* Can inject multiple objects of the same type.
* Can resolve and inject instances from types that are not bound to the container.
* Fast dependency resolution with internal cache.<a href=#performance>\*</a>
* Use of attributes to indicate injections, preferable constructors and post constructors.
* Can be easily extended through extensions.
* Framework decoupled from Unity - all Unity based API is achieved through extensions.
* Organized and well documented code written in C#.

## <a id="concepts"></a>Concepts

### <a id="what-is"></a>What is a dependency injection container?

A *dependency injection container* is a piece of software that handles the resolution of dependencies of objects. It's related to the [dependency injection](http://en.wikipedia.org/wiki/Dependency_injection) and [inversion of control](http://en.wikipedia.org/wiki/Inversion_of_control) design patterns.

The idea is that any dependency an object may need should be resolved by an external entity rather than the own object. Practically speaking, an object should not use `new` to create the objects it uses, having those instances *injected* into it by another object whose sole existence is to resolve dependencies.

So, a *dependency injection container* holds information about dependencies (the *bindings*) that can be injected into another objects by demand (injecting into existing objects) or during resolution (when you are creating a new object of some type).

### <a id="structure"></a>Structure

The structure of *Adic* is divided into five parts:

1. **InjectionContainer/Container:** binds, resolves, injects and holds dependencies. Technically, the container is a *Binder* and an *Injector* at the same time.
2. **Binder:** binds a type to another type or instance with inject conditions.
3. **Injector:** resolves and injects dependencies.
4. **Context Root:** main context in which the containers are in. Acts as an entry point for the game. It's implemented through an <a href="#extension-context-root">extension</a>.
5. **Extensions:** provides additional features to the framework.

### <a id="types-of-bindings"></a>Types of bindings

* **Transient:** a new instance is created each time a dependency needs to be resolved.
* **Singleton:** a single instance is created and used on any dependency resolution.
* **Factory:** creates the instance and returns it to the container.

### <a id="namespace-conventions"></a>Namespace conventions

*Adic* is organized internally into different namespaces that represents the framework components. However, the commonly used components are under `Adic` namespace:

1. Attributes (`Inject`, `Construct`, `PostConstruct`);
2. `InjectionContainer`;
3. `IFactory`;
4. Extensions (like `ContextRoot` and `UnityBinding`).

### <a id="chaining"></a>Chaining

Methods from the container and bindings creation can be chained to achieve a more compact code:

```cs
//Create the container.
this.AddContainer<InjectionContainer>()		
	//Register any extensions the container may use.
	.RegisterExtension<CommanderContainerExtension>()
	.RegisterExtension<EventCallerContainerExtension>()
	.RegisterExtension<UnityBindingContainerExtension>()
	//Add bindings.
    .Bind<Type1>.To<AnotherType1>()
    .Bind<Type2>.To<AnotherType2>().As("Identifier")
    .Bind<Type3>.ToSingleton<AnotherType3>();
```

**Good practice:** when chaining, always place the bindings in the end of the chain or use <a href="#bindings-setup">bindings setup</a> to organize your bindings.

## <a id="quick-start"></a>Quick start

1\. Create the context root (e.g. GameRoot.cs) of your scene by inheriting from `Adic.ContextRoot`.
   
```cs
using UnityEngine;

namespace MyNamespace {
	/// <summary>
	/// Game context root.
	/// </summary>
	public class GameRoot : Adic.ContextRoot {
		public override void SetupContainers() {
			//Setup the containers.
		}

		public override void Init() {
			//Init the game.
		}
	}
}
```

**Note:** there should be only one context root per scene.

**Hint:** when using a context root for each scene of your game, to make the project more organized, create folders for each of your scenes that will hold their own scripts and context roots.
   
2\. In the `SetupContainers()` method, create and add any containers will may need, also configuring their bindings.

```cs
public override void SetupContainers() {
	//Create a container.
	this.AddContainer<InjectionContainer>()
		//Setup bindinds.
		.Bind<Whatever>().ToSelf();
}
```

**Hint:** in *Adic*, the lifetime of your bindings is the lifetime of your containers - you can create as many containers as you want to hold your dependencies.

**Good practice:** if you have many bindings to add to a container, it's better to create reusable objects that can setup related bindings together. Please see <a href="#bindings-setup">Bindings setup</a> for more information.

3\. On the `Init()` method, place any code to start your game.

**Note:** the idea of this method is to work as an entry point for your game, like a `main()` method on console applications.

4\. Attach the context root created by you on an empty game object in your scene.

5\. Start dependency injecting!

## <a id="api"></a>API

### <a id="bindings"></a>Bindings

Binding is the action of linking a type to another type or instance. *Adic* makes it simple by providing different ways to create your bindings.

Every binding must occur from a certain key type by calling the `Bind()` method of the container. 

The simple way to bind e.g. some interface to its class implementation is as below:
   
```cs
container.Bind<SomeInterface>().To<ClassImplementation>();
```

It's also possible to bind a class to an existing instance:

```cs
container.Bind<SomeInterface>().To(someInstance);
```

You can also bind a Unity component to a game object that has that particular component:

```cs
container.Bind<Transform>().ToGameObject("GameObjectNameOnHierarchy");
```

Or a prefab on some `Prefabs/Whatever` resources folder:

```cs
container.Bind<Transform>().ToPrefab("Prefabs/Whatever/MyPrefab");
```

And, if needed, non generics versions of bindings' methods are also available:

```cs
container.Bind(someType).To(anotherType);
```

The next sections will cover all the available bindings *Adic* provides.

#### To Self

Binds the key type to a transient of itself. The key must be a class.

```cs
container.Bind<ClassType>().ToSelf();
```

#### To Singleton

Binds the key type to a singleton of itself. The key must be a class.

```cs
container.Bind<ClassType>().ToSingleton();
```

It's also possible to create a singleton of the key type to another type. In this case, the key may not be a class.

```cs
//Using generics...
container.Bind<InterfaceType>().ToSingleton<ClassType>();
//...or instance type.
container.Bind<InterfaceType>().ToSingleton(classTypeObject);
```

#### To another type

Binds the key type to a transient of another type. In this case, the *To* type will be instantiated every time a resolution of the key type is asked.

```cs
//Using generics...
container.Bind<InterfaceType>().To<ClassType>();
//..or instance type.
container.Bind<InterfaceType>().To(classTypeObject);
```

#### To instance

Binds the key type to an instance.

```cs
//Using generics...
container.Bind<InterfaceType>().To<ClassType>(instanceOfClassType);
//..or instance type.
container.Bind<InterfaceType>().To(classTypeObject, instanceOfClassType);
```

#### To all types in a namespace as transient

Binds the key type to all assignable types in a given namespace as transient bindings.

**Note 1:** it will create a <a href="#multiple-injection">multiple binding</a> if there's more than one type in the namespace that is assignable to the key type.

**Note 2:** currently it's not possible to use conditions when binding to all types in a namespace.

```cs
container.Bind<SomeType>().ToNamespace("MyNamespace.Whatever");
```

#### To all types in a namespace as singleton

Binds the key type to all assignable types in a given namespace as singleton bindings.

**Note 1:** it will create a <a href="#multiple-injection">multiple binding</a> if there's more than one type in the namespace that is assignable to the key type.

**Note 2:** currently it's not possible to use conditions when binding to all types in a namespace.

```cs
container.Bind<SomeType>().ToNamespaceSingleton("MyNamespace.Whatever");
```

#### To a Factory

Binds the key type to a factory. The factory must implement the `Adic.IFactory` interface.

```cs
//Binding factory by generics...
container.Bind<InterfaceType>().ToFactory<Factory>();
//...or type instance...
container.Bind<InterfaceType>().ToFactory(typeFactory);
//...or a factory instance.
container.Bind<InterfaceType>().ToFactory(factoryInstance);
```

See <a href="#factories">Factories</a> for more information.

#### To game object

Binds the key type to a singleton of itself or some type on a new game object.

**Good practice:** to prevent references to destroyed objects, only bind to game objects that won't be destroyed in the scene.

```cs
//Binding to itself...
container.Bind<SomeMonoBehaviour>().ToGameObject();
//...or some other component using generics...
container.Bind<SomeInterface>().ToGameObject<SomeMonoBehaviour>();
//..or some other component by instance type.
container.Bind<SomeInterface>().ToGameObject(someMonoBehaviourType);
```

The newly created game object will have the same name as the key type.

#### To game object by name

Binds the key type to a singleton `UnityEngine.Component` of itself or some type on a game object of a given name.

**Good practice:** to prevent references to destroyed objects, only bind to game objects that won't be destroyed in the scene.

If the component is not found on the game object, it will be added.

```cs
//Binding to itself by name...
container.Bind<SomeMonoBehaviour>().ToGameObject("GameObjectName");
//...or some other component using generics and name...
container.Bind<SomeInterface>().ToGameObject<SomeMonoBehaviour>("GameObjectName");
//..or some other component by instance type and name.
container.Bind<SomeInterface>()().ToGameObject(someMonoBehaviourType, "GameObjectName");
```

#### To game object with tag

Binds the key type to a singleton `UnityEngine.Component` of itself or some type on a game object of a given tag.

**Good practice:** to prevent references to destroyed objects, only bind to game objects that won't be destroyed in the scene.

If the component is not found on the game object, it will be added.

```cs
//Binding to itself by tag...
container.Bind<SomeMonoBehaviour>().ToGameObjectWithTag("Tag");
//...or some other component using generics and tag...
container.Bind<SomeInterface>().ToGameObjectWithTag<SomeMonoBehaviour>("Tag");
//..or some other component by instance type and tag.
container.Bind<SomeInterface>().ToGameObjectWithTag(someMonoBehaviourType, "Tag");

#### To game objects with tag

Binds the key type to singletons `UnityEngine.Component` of itself or some type on a game object of a given tag.

**Good practice:** to prevent references to destroyed objects, only bind to game objects that won't be destroyed in the scene.

If the component is not found on the game object, it will be added.

```cs
//Binding to itself by tag...
container.Bind<SomeMonoBehaviour>().ToGameObjectsWithTag("Tag");
//...or some other component using generics and tag...
container.Bind<SomeInterface>().ToGameObjectsWithTag<SomeMonoBehaviour>("Tag");
//..or some other component by instance type and tag.
container.Bind<SomeInterface>().ToGameObjectsWithTag(someMonoBehaviourType, "Tag");
```

#### To prefab transient

Binds the key type to a transient `UnityEngine.Component` of itself or some type on the prefab.

If the component is not found on the game object, it will be added.

**Note:** every resolution of a transient prefab will generate a new instance. So, even if the component resolved from the prefab is destroyed, it won't generate any missing references in the container.

```cs
//Binding prefab to itself...
container.Bind<SomeMonoBehaviour>().ToPrefab("Prefabs/Whatever/MyPrefab");
//...or to another component on the prefab using generics...
container.Bind<SomeInterface>().ToPrefab<SomeMonoBehaviour>("Prefabs/Whatever/MyPrefab");
//...or to another component on the prefab using instance tyoe.
container.Bind<SomeInterface>().ToPrefab(someMonoBehaviourType, "Tag");
```

#### To prefab singleton

Binds the key type to a singleton `UnityEngine.Component` of itself or some type on a newly instantiated prefab.

**Good practice:** to prevent references to destroyed objects, only bind to prefabs that won't be destroyed in the scene.

```cs
//Binding singleton prefab to itself...
container.Bind<SomeMonoBehaviour>().ToPrefabSingleton("Prefabs/Whatever/MyPrefab");
//...or to another component on the prefab using generics...
container.Bind<SomeInterface>().ToPrefabSingleton<SomeMonoBehaviour>("Prefabs/Whatever/MyPrefab");
//...or to another component on the prefab using instance type.
container.Bind<SomeInterface>().ToPrefabSingleton(someMonoBehaviourType, "Tag");
```

#### To resource

Binds the key type to a singleton `UnityEngine.Object` loaded from the Resources folder.

**Good practice:** To prevent references to destroyed objects, only bind to resources that won't be destroyed in the scene.

```cs
container.Bind<AudioClip>().ToResource("Audio/MyAudio");
```

### <a id="constructor-injection"></a>Constructor injection

*Adic* will always try to resolve any dependencies the constructor may need by using information from its bindings or trying to instantiate any types that are unknown to the binder.

**Note 1:** if there's more than one constructor, *Adic* always look for the one with less parameteres. However, <a href="#multiple-constructors">it's possible to indicate which constructor should be used</a> on a multi constructor class.

**Note 2:** there's no need to decorate constructors' parameteres with `Inject` attributes - they will be resolved automatically.

**Note 3:** currently, injection identifiers are not supported on constructors. However, <a href="#conditions">any conditions</a> (that are not identifiers) on types are also applied to the constructor parameters.

### <a id="member-injection"></a>Member injection

*Adic* can perform dependency injection on public fields and properties of classes. To make it happen, just decorate the members with the `Inject` attribute:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class summary.
	/// </summary>
	public class MyClass {
		/// <summary>Field to be injected.</summary>
		[Inject]
		public SomeClass fieldToInject;
		/// <summary>Field NOT to be injected.</summary>
		public SomeClass fieldNotToInject;

		/// <summary>Property to be injected.</summary>
		[Inject]
		public SomeOtherClass propertyToInject { get; set; }		
		/// <summary>Property NOT to be injected.</summary>
		public SomeOtherClass propertyNotToInject { get; set; }
	}
}
```

If you need to perform actions after all injections have been completed, create a method and decorate it with the `PostConstruct` attribute:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class summary.
	/// </summary>
	public class MyClass {
		/// <summary>Field to be injected.</summary>
		[Inject]
		public SomeClass fieldToInject;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public MyClass() {
			...
		}

		/// <summary>
		/// Class post constructor, called after all the dependencies have been resolved.
		/// </summary>
		[PostConstruct]
		public void PostConstruct() {
			...
		}
	}
}
```

### <a id="multiple-constructors"></a>Multiple constructors

In case you have multiple constructors, it's possible to indicate to *Adic* which one should be used by decorating it with the `Construct` attribute:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class summary.
	/// </summary>
	public class MyClass {
		/// <summary>
		/// Class constructor.
		/// </summary>
		public MyClass() {
			...
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="parameterName">Parameter description</param>
		[Construct]
		public MyClass(Type parameterName) {
			...
		}
	}
}
```

### <a id="multiple-injection"></a>Multiple injection

It's possible to inject multiple objects of the same type by creating a series of bindings of the same key type. In this case, the injection occurs on an array of the key type.

Binding multiple objects to the same key:

```cs
container
	.Bind<GameObject>().ToGameObject("Enemy1")
	.Bind<GameObject>().ToGameObject("Enemy2")
	.Bind<GameObject>().ToGameObject("Enemy3")
	.Bind<GameObject>().ToGameObject("Enemy4")
	.Bind<GameObject>().ToGameObject("Enemy5");
```

Multiple injection in a field:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class summary.
	/// </summary>
	public class MyClass {
		/// <summary>Enemies on the game.</summary>
		[Inject]
		public GameObject[] enemies;
	}
}
```

It's possible to manually resolve multiple objects. Please see <a href="#manual-type-resolution">Manual type resolution</a> for more information.

### <a id="monobehaviour-injection"></a>MonoBehaviour injection

It's possible to perform injection on custom `MonoBehaviour` fields and properties by using the <a href="#extension-mono-injection">Mono Injection</a> extension, which is enabled by default, by calling `this.Inject()` on the `Start()` method of the `MonoBehaviour`:

```cs
using Unity.Engine;

namespace MyNamespace {
	/// <summary>
	/// My MonoBehaviour summary.
	/// </summary>
	public class MyBehaviour : MonoBehaviour {
		/// <summary>Field to be injected.</summary>
		[Inject]
		public SomeClass fieldToInject;

		protected void Start() {
			this.Inject();
		}
	}
}
```

#### Using a base MonoBehaviour

To make injection even simpler, create a base behaviour from which all your `MonoBehaviour` will inherit:

```cs
using Unity.Engine;

namespace MyNamespace {
	/// <summary>
	/// Base MonoBehaviour.
	/// </summary>
	public abstract class BaseBehaviour : MonoBehaviour {
		/// <summary>
		/// Called when the component is starting.
		/// 
		/// If overriden on derived classes, always call base.Start().
		/// </summary>
		protected virtual void Start() {
			this.Inject();
		}
	}
}
```

#### Injecting from multiple containers

When injecting into `MonoBehaviour` using the `this.Inject()` method, every available container in the <a href="#quick-start">context root</a> is used. If you want to restrict the containers from which injection occurs, use the `InjectFromContainer` attribute in conjunction with a container identifier.

##### Setting a container identifier

When creating the container, set an identifier through its constructor:

```cs
//Create the container with an identifier.
this.AddContainer(new InjectionContainer("identifier"))
	//Register any extensions the container may use.
	.RegisterExtension<UnityBindingContainerExtension>();
```

##### Adding the attribute

In the `MonoBehaviour` that should receive injection only from a certain container, add the `InjectFromContainer` attribute with the container's identifier:

```cs
using Unity.Engine;

namespace MyNamespace {
	/// <summary>
	/// My MonoBehaviour summary.
	/// </summary>
	[InjectFromContainer("identifier")]
	public class MyBehaviour : MonoBehaviour {
		/// <summary>Field to be injected.</summary>
		[Inject]
		public SomeClass fieldToInject;

		protected void Start() {
			this.Inject();
		}
	}
}
```

### <a id="conditions"></a>Conditions

Conditions allow a more customized approach when injecting dependencies into constructors and fields/properties.

Using conditions you can:

1\. Tag a binding with an identifier, so you can indicate it as a parameter in the `Inject` attribute on fields/properties:

When binding:

```cs
container.Bind<SomeInterface>().To<SomeClass>().As("Identifier");
```

When injecting:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class summary.
	/// </summary>
	public class MyClass {
		/// <summary>Field to be injected.</summary>
		[Inject("Identifier")]
		public SomeInterface field;
	}
}
```

2\. Indicate in which objects a binding can be injected, by type or instance:

```cs
//Using generics...
container.Bind<SomeInterface>().To<SomeClass>().WhenInto<MyClass>();
//...or type instance...
container.Bind<SomeInterface>().To<SomeClass>().WhenInto(myClassInstanceType);
//...or by a given instance.
container.Bind<SomeInterface>().To<SomeClass>().WhenIntoInstance(myClassInstanceType);
```

3\. Create complex conditions by using an anonymous method:

```cs
container.Bind<SomeInterface>().To<SomeClass>().When(context => 
		context.member.Equals(InjectionMember.Field) &&
        context.parentType.Equals(typeof(MyClass))
	);
```

The context provides the following fields:

1. **member** (`Adic.InjectionMember` enum): the class member in which the injection is occuring (*None*, *Constructor*, *Field* or *Property*).
2. **memberType** (`System.Type`): the type of the member in which the injection is occuring.
3. **identifier** (`string`): the identifier of the member in which the injection is occuring (from `Inject` attribute).
4. **parentType** (`System.Type`): the type of the object in which the injection is occuring.
5. **parentInstance** (`object`): the instance of the object in which the injection is occuring.
6. **injectType** (`System.Type`): the type of the object being injected.

### <a id="update"></a>Update

It's possible to have an `Update()` method on regular classes (that don't inherit from `MonoBehaviour`) by implementing the `Adic.IUpdatable` interface.

See <a href="#extension-event-caller">Event Caller</a> for more information.

### <a id="dispose"></a>Dispose

When a scene is destroyed, it's possible to have a method that can be called to e.g. free up resources. To do it, implement the `System.IDisposable` interface on any class that you want to have this option.

See <a href="#extension-event-caller">Event Caller</a> for more information.

### <a id="manual-type-resolution"></a>Manual type resolution

If you need to get a type from the container but do not want to use injection through constructor or fields/properties, it's possible to execute a manual resolution directly by calling the `Resolve()` method:

```cs
//Resolving using generics...
var instance = container.Resolve<Type>();
//...or using generics for objects with a given identifier...
var instance = container.Resolve<Type>("Identifier");
//...or by type instance...
instance = container.Resolve(typeInstance);
//...or by objects with a given identifier...
instance = container.Resolve("Identifier");
//...or by type instance for objects with a given identifier.
instance = container.Resolve(typeInstance, "Identifier");
```

It's also possible to resolve all objects of a given type:

```cs
//Resolving all objects using generics...
var instances = container.ResolveAll<Type>();
//...or using generics for objects with a given identifier...
var instance = container.ResolveAll<Type>("Identifier");
//...or by type instance...
instances = container.ResolveAll(typeInstance);
//...or by objects with a given identifier...
instance = container.ResolveAll("Identifier");
//...or by type instance for objects with a given identifier.
instance = container.ResolveAll(typeInstance, "Identifier");
```

**Note:** although it's possible to resolve instances by identifier, currently manual resolution of bindings that have other conditions is not supported.

### <a id="factories"></a>Factories

When you need to handle the instantiation of an object manually, it's possible to create a factory class by inheriting from `Adic.IFactory`:

```cs
using Adic.Injection;

namespace MyNamespace {
	/// <summary>
	/// My factory.
	/// </summary>
	public class MyFactory : Adic.IFactory {
		/// <summary>
		/// Creates an instance of the object of the type created by the factory.
		/// </summary>
		/// <param name="context">Injection context.</param>
		/// <returns>The instance.</returns>
		public object Create(InjectionContext context) {
			//Instantiate and return the object.
			var myObject = new MyObject();
			return myObject;
		}
	}
}
```

The `InjectionContext` object contains information about the current injection/resolution, which can be used to help deciding how the instance will be created by the factory.

To bind a type to a factory class, use the `ToFactory()`:

```cs
//Binding factory by generics...
container.Bind<SomeType>().ToFactory<MyFactory>();
//...or type instance...
container.Bind<SomeType>().ToFactory(typeMyFactory);
//...or a factory instance.
container.Bind<SomeType>().ToFactory(factoryInstance);
```

**Note:** factories are resolved and injected by the container. So, it's possible to receive dependencies either by construtor and/or fields/properties.

### <a id="bindings-setup"></a>Bindings setup

Sometimes the bindings list can become (very) large and bloat the `SetupContainers()` method of the context root. For better organization, it's possible to create reusable objects which will group and setup related bindings in a given container.

To create a bindings setup object, implement the `Adic.IBindingsSetup` interface:

```cs
using Adic;
using Adic.Container;

namespace MyNamespace.Bindings { 
	/// <summary>
	/// My bindings.
	/// </summary>
	public class MyBindings : IBindingsSetup {
		public void SetupBindings (IInjectionContainer container) {
			container.Bind<SomeType>().ToSingleton<AnotherType>();
			//...more related bindings.
		}
	}
}
```

To perform a bindings setup, call the `SetupBindings()` method in the container, passing either the binding setup object as a parameter or the namespace in which the setups reside:

```cs
//Setup by generics...
container.SetupBindings<MyBindings>();
//...or by type...
container.SetupBindings(typeof(MyBindings));
//...or from an instance...
var bindings = MyBindings();
container.SetupBindings(bindings);
//...or using a namespace.
container.SetupBindings("MyNamespace.Bindings");
```

**Note:** the default behaviour of `SetupBindings()` with namespace is to use all `IBindingsSetup` objects under the given namespace and all its children namespaces. If you need that only `IBindingsSetup` objects in the given namespace are used, call the overload that allows indication of children namespace evaluation:

```cs
container.SetupBindings("MyNamespace.Bindings", false);
```

#### Binding setup priorities

The order of bindings setups matters. In case an `IBindingsSetup` object relies on bindings from another `IBindingsSetup` object, add the other setup first.

However, if you are using `SetupBindings()` with a namespace, it's not possible to manually order the `IBindingsSetup` objects. In this case, you have to decorate the `IBindingsSetup` classes with a `BindingPriority` attribute to define the priority in which each bindings setup will be executed:

```cs
using Adic;
using Adic.Container;

namespace MyNamespace.Bindings { 
	/// <summary>
	/// My bindings.
	/// </summary>
	[BindingPriority(1)]
	public class MyBindings : IBindingsSetup {
		public void SetupBindings (IInjectionContainer container) {
			container.Bind<SomeType>().ToSingleton<AnotherType>();
			//...more related bindings.
		}
	}
}
```

Higher values indicate higher priorities. If no priority value is provided, the default value of `1` will be used.

### <a id="using-commands"></a>Using commands

#### What are commands?

Commands are actions executed by your game, usually in response to an event.

The concept of commands on *Adic* is to place everything the action/event needs in a single place, so it's easy to understand and maintain it.

Suppose you have an event of enemy destroyed. When that occurs, you have to update UI, dispose the enemy, spawn some particles and save statistics somewhere. One approach would be dispatch the event to every object that has to do something about it, which is fine given it keeps single responsibility by allowing every object to take care of their part on the event. However, when your project grows, it can be a nightmare to find every place a given event is handled. That's when commands come in handy: you place all the code and dependencies for a certain action/event in a single place.

#### Creatings commands

To create a command, inherit from `Adic.Command` and override the `Execute()` method, where you will place all the code needed to execute the command. If you have any dependencies to be injected before executing the command, add them as fields or properties and decorate them with an `Inject` attribute:

```cs
namespace MyNamespace.Commands {
	/// <summary>
	/// My command.
	/// </summary>
	public class MyCommand : Adic.Command {
		/// <summary>Some dependency to be injected.</summary>
		[Inject]
		public object someDependency;

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters">Command parameters.</param>
		public override void Execute(params object[] parameters) {
			//Execution code.
		}
	}
}
```
**Hint:** it's also possible to wire any dependencies through constructor. However, in this case the dependencies will only be resolved once, during instantiation.

**Good practice:** place all your commands under the same namespace, so it's easy to register them.

##### Types of commands

###### Pooled

The command is kept in a pool for reuse, avoiding new instantiations. It's useful for commands that need to maintain state when executing. This is the default behaviour.

When creating pooled commands, it's possible to set the initial and maximum size of the pool for a particular command by setting, respectively, the `preloadPoolSize` and `maxPoolSize` properties:

```cs
namespace MyNamespace.Commands {
	/// <summary>
	/// My command.
	/// </summary>
	public class MyCommand : Adic.Command {
		/// <summary>The quantity of the command to preload on pool (default: 1).</summary>
		public override int preloadPoolSize { get { return 5; } }
		/// <summary>The maximum size pool for this command (default: 10).</summary>
		public override int maxPoolSize { get { return 20; } }

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters">Command parameters.</param>
		public override void Execute(params object[] parameters) {
			//Execution code.
		}
	}
}
```

###### Singleton

There's only one copy of the command available, which is used every time the command is dispatched. It's useful for commands that don't need state or every dependency the command needs is given during execution. To make a command singleton, return `true` in the `singleton` property of the command:

```cs
namespace MyNamespace.Commands {
	/// <summary>
	/// My command.
	/// </summary>
	public class MyCommand : Adic.Command {
		/// <summary>Indicates whether this command is a singleton (there's only one instance of it).</summary>
		public override bool singleton { get { return true; } }

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters">Command parameters.</param>
		public override void Execute(params object[] parameters) {
			//Execution code.
		}
	}
}
```

**Note:** When using singleton commands, injection is done only through constructors or injection after command instantiation.

#### Registering commands

To register a command, call the `Register()` method on the container, usually in the context root:

```cs
using UnityEngine;

namespace MyNamespace {
	/// <summary>
	/// Game context root.
	/// </summary>
	public class GameRoot : Adic.ContextRoot {
		public override void SetupContainers() {
			//Create the container.
			this.AddContainer<InjectionContainer>()
				//Register any extensions the container may use.
				.RegisterExtension<CommanderContainerExtension>()
				//Registering by generics...
				.RegisterCommand<MyCommand>()
				//...or by type.
				.RegisterCommand(typeof(MyCommand));
		}

		public override void Init() {
			//Init the game.
		}
	}
}
```

**Note:** when registering a command, it's placed in the container, so it's easier to resolve it and its dependencies.

It's also possible to register all commands under the same namespace by calling the `RegisterCommands()` method in the container and passing the full name of the namespace:

```cs
public override void SetupContainers() {
	//Create the container.
		this.AddContainer<InjectionContainer>()
			//Register any extensions the container may use.
			.RegisterExtension<CommanderContainerExtension>()
			//Register all commands under the namespace "MyNamespace.Commands".
			.RegisterCommands("MyNamespace.Commands");
}
```

**Note:** the default behaviour of `RegisterCommands()` is to register all commands under the given namespace and all its children namespaces. If you need that only commands in the given namespace are registered, call the overload that allows indication of children namespace evaluation:

```cs
container.RegisterCommands("MyNamespace.Commands", false);
```

#### Dispatching commands

##### From code

To dispatch a command, just call the `Dispatch()` method on `Adic.ICommandDispatcher`, using either the generics or the by `System.Type` versions:

```cs
/// <summary>
/// My method that dispatches a command.
/// </summary>
public void MyMethodThatDispatchesCommands() {
	//Dispatching by generics...
	dispatcher.Dispatch<MyCommand>();
	//...or by type.
	dispatcher.Dispatch(typeof(MyCommand));
}
```

It's also possible to dispatch a command after a given period of time by calling the `InvokeDispatch()` method:

```cs
//Timed dispatching by generics...
dispatcher.InvokeDispatch<MyCommand>(1.0f);
//...or by type.
dispatcher.InvokeDispatch(typeof(MyCommand), 1.0f);
```

To use `Adic.ICommandDispatcher`, you have to inject it wherever you need to use it:

```cs
namespace MyNamespace {
	/// <summary>
	/// My class that dispatches commands.
	/// </summary>
	public class MyClassThatDispatchesCommands {
		/// <summary>The command dispatcher.</summary>
		[Inject]
		public ICommandDispatcher dispatcher;

		/// <summary>
		/// My method that dispatches a command.
		/// </summary>
		public void MyMethodThatDispatchesCommands() {
			this.dispatcher.Dispatch<MyCommand>();
		}
	}
}
```

**Hint:** commands already have a reference to its dispatcher (`this.dispatcher`).

**Note 1:** when dispatching a command, it's placed in a list in the command dispatcher object, which is the one responsible for pooling and managing existing commands.

**Note 2:** commands in the pool that are not singleton are *reinjected* every time they are executed.

##### From game objects

It's possible to dispatch commands directly from game objects without the need to write any code using the components available in the <a href="#extension-commander">Commander extension</a>.

To use them, just add the desired component to a game object.

###### Command Dispatch

Provides a routine to call a given command. The routine name is `DispatchCommand()`.

Using this component, you can e.g. call the `DispatchCommand()` method from a button in the UI or in your code.

It can be found at the `Component/Adic/Commander/Command dispatch` menu.

###### Timed Command Dispatch

Dispatches a command based on a timer.

This component also provides the `DispatchCommand()` routine, in case you want to call it before the timer ends.

It can be found at the `Component/Adic/Commander/Timed command dispatch` menu.

#### Retaining commands

When a command needs to continue its execution beyond the `Execute()` method, it has to be retained. This way the command dispatcher knows the command should only be pooled/disposed when it finishes its execution.

This is useful not only for commands that implement `Adic.IUpdatable`, but also for commands that have to wait until certain actions (e.g. some network call) are completed.

To retain a command, just call the `Retain()` method during main execution:

```cs
/// <summary>
/// Executes the command.
/// </summary>
/// <param name="parameters">Command parameters.</param>
public override void Execute(params object[] parameters) {
	//Execution code.

	//Retains the command until some long action is completed.
	this.Retain();
}
```

If a command is retained, it has to be released. The command dispatcher will automatically releases commands during the destruction of scenes. However, in some situations you may want to release it manually (e.g. after some network call is completed).

To release a command, just call the `Release()` method when the execution is finished:

```cs
/// <summary>
/// Called when some action is finished.
/// </summary>
public void OnSomeActionFinished() {
	//Releases the command.
	this.Release();
}
```

#### Timed invoke

It's possible to use timed method invocation inside a command by calling the `Invoke()` method:

```cs
namespace MyNamespace.Commands {
	/// <summary>
	/// My command.
	/// </summary>
	public class MyCommand : Adic.Command {
		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters">Command parameters.</param>
		public override void Execute(params object[] parameters) {
			//Invokes the "MyMethodToInvoke" method after 1 second.
			this.Invoke(this.MyMethodToInvoke, 1.0f);

			//Retains the command until the invocation is finished.
			this.Retain();
		}

		/// <summary>
		/// My method to invoke.
		/// </summary>
		private void MyMethodToInvoke() {
			//Method code.

			//Releases the command after the invocation.
			this.Release();
		}
	}
}
```

**Note:** always retain a command when invoking methods inside it.

#### Coroutines

It's possible to use [coroutines](http://docs.unity3d.com/Manual/Coroutines.html) inside a command by calling the `StartCoroutine()` method:

```cs
namespace MyNamespace.Commands {
	/// <summary>
	/// My command.
	/// </summary>
	public class MyCommand : Adic.Command {
		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters">Command parameters.</param>
		public override void Execute(params object[] parameters) {
			//Starts the coroutine.
			this.StartCoroutine(this.MyCoroutine());

			//Retains the command until the coroutine is finished.
			this.Retain();
		}

		/// <summary>
		/// My coroutine.
		/// </summary>
		private IEnumerator MyCoroutine() {
			//Coroutine code.

			//Releases the command after execution.
			this.Release();
		}
	}
}
```

If needed, it's also possible to stop a coroutine after it's started by calling the `StopCoroutine()` method.

**Note:** always retain a command when using coroutines.

#### A note about scene destruction and commands

When a scene is destroyed, all commands will be released and all registrations will be disposed.

So, if you're using a <a href="#static-containers">container that will live through scenes</a>, be aware that all commands will have to be registered again.

## <a id="order-of-events"></a>Order of events

1. Unity Awake()
2. ContextRoot calls SetupContainers()
3. ContextRoot asks for each container to generate cache for its types
4. ContextRoot calls Init()
5. Unity Start() on all `MonoBehaviour`
6. Injection on `MonoBehaviour`
7. Update() is called on every object that implements `Adic.IUpdatable`
8. Scene is destroyed
9. Dispose() is called on every object that implements `System.IDisposable`

## <a id="performance"></a>Performance

*Adic* was created with speed in mind, using internal cache to minimize the use of [reflection](http://en.wikipedia.org/wiki/Reflection_%28computer_programming%29) (which is usually slow), ensuring a good performance when resolving and injecting into objects - the container can resolve a 1.000 objects in 1ms<a href="#about-performance-tests">\*</a>.

To maximize performance, always bind all types that will be resolved/injected in the <a href="#quick-start">ContextRoot</a>, so *Adic* can generate cache of the objects and use that information during runtime.

If you have more than one container on the same scene, it's possible to share the cache between them. To do so, create an instance of `Adic.Cache.ReflectionCache` and pass it to any container you create:

```cs
using UnityEngine;

namespace MyNamespace {
	/// <summary>
	/// Game context root.
	/// </summary>
	public class GameRoot : Adic.ContextRoot {
		public override void SetupContainers() {
			//Create the reflection cache.
			var cache = new ReflectionCache();

			//Create a new container.
			var container1 = this.AddContainer(new InjectionContainer(cache));

			//Container configurations and bindings...

			//Create a new container.
			var container2 = this.AddContainer(new InjectionContainer(cache));

			//Container configurations and bindings...
		}

		public override void Init() {
			//Init the game.
		}
	}
}
```

<sup><a id="about-performance-tests">\*</a> See *Tests/Editor/SpeedTest.cs* for more details on performance tests. Tested on a MacBook Pro late 2014 (i7 2.5/3.7 GHz).</sup>

<sup>\- A thousand simple resolves in 1ms</sup><br>
<sup>\- A million simple resolves in 1330ms</sup><br>
<sup>\- A thousand complex resolves in 2ms</sup><br>
<sup>\- A million complex resolves in 2428ms</sup>

<sup>A *simple resolve* is the resolution of a class without any `Inject` attributes.</sup><br>
<sup>A *complex resolve* is the resolution of a class that is not bound to the container and has a `Inject` attribute in a field.</sup>

## <a id="container-extensions"></a>Extensions

Extensions are a way to enhance *Adic* without having to edit it to suit different needs. By using extensions, the core of *Adic* is kept agnostic, so it can be used on any C# environment.

## <a id="available-extensions"></a>Available extensions

### <a id="extension-bindings-printer"></a>Bindings Printer

Prints all bindings on any containers on the current `ContextRoot`. It must be executed on Play Mode.

To open the Bindings Printer windows, click on *Windows/Adic/Bindings Printer* menu.

#### Format

```
[Container Type Full Name] (index: [Container Index on ContextRoot], [destroy on load/singleton])

	[For each binding]
	Type: [Binding Type Full Name]
	Bound to: [Bound To Type Full Name] ([type/instance])
	Binding type: [Transient/Singleton/Factory]
	Identifier [Identifier/-]
	Conditions: [yes/no]
```

#### Dependencies

* <a href="#extension-context-root">Context Root</a>

### <a id="extension-bindings-setup"></a>Bindings Setup

Provides a convenient place to setup bindings and reuse them in different containers and scenes.

#### Configuration

Please see <a href="#bindings-setup">Bindings setup</a> for more information.

#### Dependencies

None

### <a id="extension-commander"></a>Commander

Provides dispatching of commands, with pooling for better performance.

For more information on commands, see <a href="#using-commands">Using commands</a>.

#### Configuration

Register the extension on any containers that will use it:

```cs
//Create the container.
this.AddContainer<InjectionContainer>()
	//Register any extensions the container may use.
	.RegisterExtension<CommanderContainerExtension>();
```

If you use `IDiposable` or `IUpdatable` events, also register the <a href="#extension-event-caller">Event Caller</a> extension:

```cs
//Create the container.
this.AddContainer<InjectionContainer>()
	//Register any extensions the container may use.
	.RegisterExtension<CommanderContainerExtension>()
	.RegisterExtension<EventCallerContainerExtension>();
```

#### Dependencies

* <a href="#extension-event-caller">Event Caller</a>

### <a id="extension-context-root"></a>Context Root

Provides an entry point for the game on Unity 3D.

#### Configuration

Please see <a href="#quick-start">Quick start</a> for more information.

#### <a id="static-containers"></a>Notes

1. When adding containers using `AddContainer()`, it's possible to keep them alive between scenes by setting the `destroyOnLoad` to `false`.

#### Dependencies

None

### <a id="extension-event-caller"></a>Event Caller

Calls events on classes that implement certain interfaces. The classes must be bound to a container.

#### Available events

##### Update

Calls `Update()` method on classes that implement `Adic.IUpdatable` interface.

```cs
namespace MyNamespace {
	/// <summary>
	/// My updateable class.
	/// </summary>
	public class MyUpdateableClass : Adic.IUpdatable {
		public void Update() {
			//Update code.
		}
	}
}
```

##### Dispose

When a scene is destroyed, calls `Dispose()` method on classes that implement `System.IDisposable` interface.

```cs
namespace MyNamespace {
	/// <summary>
	/// My disposable class.
	/// </summary>
	public class MyDisposableClass : System.IDisposable {
		public void Dispose() {
			//Dispose code.
		}
	}
}
```

#### Configuration

Register the extension on any containers that will use it:

```cs
//Create the container.
this.AddContainer<InjectionContainer>()
	//Register any extensions the container may use.
	.RegisterExtension<EventCallerContainerExtension>();
```

#### Notes

1. Currently, any objects that are updateable are not removed from the update's list when they're not in use anymore. So, it's recommended to implement the `Adic.IUpdatable` interface only on singleton or transient objects that will live until the scene is destroyed;
2. When the scene is destroyed, the update's list is cleared. So, any objects that will live between scenes that implement the `Adic.IUpdatable` interface will not be readded to the list. **It's recommeded to use updateable objects only on the context of a single scene**.
3. Be aware of singleton objects on containers that will live through scenes. Eventually these objects may try to use references that may not exist anymore.

#### Dependencies

None

### <a id="extension-mono-injection"></a>Mono Injection

Allows injection on `MonoBehaviour` by provinding an `Inject()` method.

#### Configuration

Please see <a href="#monobehaviour-injection">MonoBehaviour injection</a> for more information.

#### Dependencies

* <a href="#extension-context-root">Context Root</a>

### <a id="extension-unity-binding"></a>Unity Binding

Provides Unity 3D bindings to the container.

Please see <a href="#bindings">Bindings</a> for more information.

#### Configuration

Register the extension on any containers that you may use it:

```cs
//Create the container.
this.AddContainer<InjectionContainer>()
	//Register any extensions the container may use.
	.RegisterExtension<UnityBindingContainerExtension>();
```

#### Notes

1. **ALWAYS CALL `Inject()` FROM 'Start'**! (use the <a href="#extension-mono-injection">Mono Injection</a> Extension).

#### Dependencies

None

## <a id="creating-extensions"></a>Creating extensions

Extensions on *Adic* can be created in 3 ways:

1. Creating a framework extension extending the base APIs through their interfaces;
2. Creating extension methods to any part of the framework;
3. Creating a container extension, which allows for the interception of internal events, which can alter the inner workings of the framework.

**Note:** always place the public parts of extensions into *Adic* namespace.

To create a *container extension*, which can intercept internal *Adic* events, you have to:

1\. Create the extension class with `ContainerExtension` sufix.

2\. Implement `Adic.Container.IContainerExtension`.

3\. Subscribe to any events on the container on OnRegister method.

```cs
public void OnRegister(IInjectionContainer container) {
	container.beforeAddBinding += this.OnBeforeAddBinding;
}
```

4\. Unsubscribe to any events the extension may use on the container on OnUnregister method.

```cs
public void OnUnregister(IInjectionContainer container) {
	container.beforeAddBinding -= this.OnBeforeAddBinding;
}
```

## <a id="container-events"></a>Container events

Container events provide a way to intercept internal actions of the container and change its inner workings to suit the needs of your extension.

All events are available through `Adic.InjectionContainer`.

### <a id="binder-events"></a>Binder events

* `beforeAddBinding`: occurs before a binding is added.
* `afterAddBinding`: occurs after a binding is added.
* `beforeRemoveBinding`: occurs before a binding is removed.
* `afterRemoveBinding`: occurs after a binding is removed.

### <a id="injector-events"></a>Injector events

* `beforeResolve`: occurs before a type is resolved.
* `afterResolve`: occurs after a type is resolved.
* `bindingEvaluation`: occurs when a binding is available for resolution.
* `bindingResolution`: occures when a binding is resolved to an instance.
* `beforeInject`: occurs before an instance receives injection.
* `afterInject`: occurs after an instance receives injection.

## <a id="general-notes">General notes</a>

1. If an instance is not found, it will be resolved to NULL.
2. Multiple injections must occur in an array of the desired type.
3. Order of bindings is controlled by just reordering the bindings during container setup.
4. Avoid singleton bindings of objects that will be destroyed during execution. This can lead to missing references in the container.
5. Any transient object, once resolved, is not tracked by *Adic*. So, if you want e.g. a list of all prefabs that were resolved by the container, you'll have to store it manually. Singleton objects are kept inside the container, given there is just a single instance of them.
6. *Adic* relies on Unity Test Tools for unit testing. You can download it at [Unity Asset Store](https://www.assetstore.unity3d.com/#!/content/13802).

## <a id="examples"></a>Examples

There are some examples that are bundled to the main package that teach the basics and beyond of *Adic*.

### 1. Hello World

Exemplifies the basics of how to setup a scene for dependency injection using the ContextRoot.

### 2. Binding Game Objects

Exemplifies how to bind components to new and existing game objects and allows them to share dependencies.

### 3. Using conditions

Exemplifies the use of condition identifiers on injections.

### 4. Prefabs

Exemplifies how to bind to prefabs and the use of `PostConstruct` as a second constructor.

### 5. Commander

Exemplifies the use of commands through a simple spawner of a prefab.

### 6. Bindings Setup

Exemplifies the use of bindings setups to better organize the bindings for a container.

### 7. Factory

Exemplifies the use of a factory to create and position cubes as a matrix.

## <a id="changelog"></a>Changelog

Please see [CHANGELOG.txt](src/Assets/Adic/CHANGELOG.txt).

## <a id="support"></a>Support

Found a bug? Please create an issue on the [GitHub project page](https://github.com/intentor/adic/issues) or send a pull request if you have a fix or extension.

You can also send me a message at support@intentor.com.br to discuss more obscure matters about *Adic*.

## <a id="license"></a>License

Licensed under the [The MIT License (MIT)](http://opensource.org/licenses/MIT). Please see [LICENSE](LICENSE) for more information.
