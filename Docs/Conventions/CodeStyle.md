# Code style

> Summary: The order of members inside every class and struct, one type per file, naming. A hook checks the member order after every C# edit Claude makes (`.claude/hooks/check_member_order.py`).

## Member order
1. **Constants and static members** (`const`, `static readonly`, static properties)
2. **Events** (`event`, public delegate fields)
3. **Serialized fields** (`[SerializeField]`, `[SerializeReference]`, `[field: SerializeField]` properties)
4. **Other fields** (readonly fields first is preferred, not checked)
5. **Properties**
6. **Constructors, `[Inject] Construct` and `Init`**
7. **Unity messages**, in lifecycle order: `Awake`, `OnEnable`, `Reset`, `Start`, `FixedUpdate`, `Update`, `LateUpdate`, `OnDisable`, `OnDestroy`; the others (`OnValidate`, `OnTrigger*`, ...) after them
8. **Public, internal and protected methods** (NUnit tests and `[SetUp]` included)
9. **Private methods**

Nested types may stand anywhere; a private serializable document type usually sits at the top, next to the key it is saved under.

```csharp
public sealed class EnemyView : MonoBehaviour
{
    private const float FadeTime = 0.2f;              // 1

    public event Action<EnemyView> Died;               // 2

    [SerializeField] private Renderer _body;           // 3

    private EnemySettings _settings;                   // 4
    private int _health;

    public bool IsAlive => _health > 0;                // 5

    public void Init(EnemySettings settings)           // 6
    {
        _settings = settings;
        _health = settings.MaxHealth;
    }

    private void OnDisable() => _body.enabled = false; // 7

    public void Hit(int damage)                        // 8
    {
        _health -= damage;
        if (!IsAlive)
            Die();
    }

    private void Die() => Died?.Invoke(this);          // 9
}
```

## Naming and other rules
- One top-level type per file (class, struct, interface, enum, delegate), and the file is named after it. Nested types stay in their owner's file. Test doubles follow the same rule.
- Private fields `_camelCase`; everything else `PascalCase`; locals and parameters `camelCase`.
- Classes not designed for inheritance are `sealed`.
- One constructor; no `[Inject]` on constructors. `[Inject]` methods only on MonoBehaviours.
- No static singletons or service locators: dependencies arrive through the constructor (or `Init` / `Construct`).
- Comments only for what the code cannot say (a non-obvious reason or invariant).

## The hook
`check_member_order.py` runs after every Edit or Write of a `.cs` file under the project. When the order is wrong it returns the problems to Claude, which fixes the file in the same turn. It reads declarations line by line, so unusual syntax can confuse it; run it by hand on a file to see what it found:

```bash
python3 .claude/hooks/check_member_order.py Assets/Path/To/File.cs
```
