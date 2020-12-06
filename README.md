# Object Pooling for Unity

## Features
- Faster in terms of performance than Instantiate/Destroy (Test at the end of README)
- Easy to use
- Easy to integrate with already written spawn systems
- Callbacks OnSpawn & OnDespawn for resseting after object being used

## How to Install
### Git Installation

If you have Git on your computer, you can open Package Manager indside Unity, select "Add package from Git url...", and paste link ```https://github.com/IntoTheDev/Object-Pooling-for-Unity.git```

or

Open the manifest.json file of your Unity project.
Add ```"com.intothedev.objectpooling": "https://github.com/IntoTheDev/Object-Pooling-for-Unity.git"```

### Manual Installation
Download latest package from the Release section
Import ObjectPooling.unitypackage to your Unity Project

## Usage
### How to populate pool:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	private void Awake()
	{
		_prefab.Populate(count: 50);
	}
}
```

Also, you can just put PoolInstaller component on any object on Scene and select which objects you want to prepopulate

![](https://i.imgur.com/gnyZ0RQ.png)

### How to spawn object from pool:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	public void Spawn()
	{
		_prefab.Spawn(transform.position, transform.rotation);
		
		// Get object from pool with component
		_prefab.Spawn<Rigidbody>(transform.position, transform.rotation).isKinematic = true;
	}
}
```

### How to return object to pool:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	public void Spawn()
	{
		var instance = _prefab.Spawn(transform.position, transform.rotation);
		
		// Use this only with Instances of Prefab
		instance.Despawn();
	}
}
```

### How to use callbacks:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Health : MonoBehaviour, IPoolable
{
	[SerializeField] private float _maxHealth = 100f;
	private float _health = 0f;
	
	private void Awake() =>
		_health = _maxHealth;
		
	// IPoolable method
	public void OnSpawn() =>
		_health = _maxHealth;
		
	// IPoolable method
	public void OnDespawn() { }
}
```

### Peformance test:
Creating and destroying 1000 objects.

#### Instantiate/Destroy:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private GameObject _object = null;
	
	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		for (int i = 0; i < 1000; i++)
		{
			var instance = Instantiate(_object);
			Destroy(instance);
		}
		
		stopwatch.Stop();
		print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
	}
}
```
##### Result: [16:26:15] Milliseconds: 6

#### Spawn/Despawn:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using ToolBox.Pools;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private GameObject _object = null;
	
	private void Awake()
	{
		_object.Populate(1000);
	}
	
	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		for (int i = 0; i < 1000; i++)
		{
			var instance = _object.Spawn();
			instance.Despawn();
		}
		
		stopwatch.Stop();
		print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
	}
}
```
##### Result: [16:29:36] Milliseconds: 2
