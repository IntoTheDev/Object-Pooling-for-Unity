# Object-Pooling-for-Unity
Object Pooling for Unity

## Features
- Faster in terms of performance than Instantiate/Destroy
- Easy to use
- Easy to integrate with already written spawn systems
- Callbacks OnSpawn & OnDespawn for resseting after object being used

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
		// Use this only with Prefabs
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
		// Use this only with Prefabs
		_prefab.Spawn(transform.position, transform.rotation);

		// Use this only with Prefabs
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
		// Use this only with Prefabs
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
