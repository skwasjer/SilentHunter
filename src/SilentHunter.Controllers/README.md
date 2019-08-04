# Silent Hunter controllers


# Controllers

There are several base controller types:

- `Controller`
- `BehaviorController`
- `AnimationController`
- `MeshAnimationController`
- `StateMachineController`

Most controllers typically inherit from `Controller`, as these describe behavior, environment, etc. The other controllers have some more specific serialization logic, and as such there is no real flexibility to modifying these.

## `Controller` serialization logic

### Accepted field types
 - primitives (bool, sbyte, byte, short, ushort, int, uint, long, ulong, float, double)
 - string
 - custom value types

### Field attributes

- `FixedStringAttibute` dictates that a string field is of fixed length. If shorter, the string will be padded with `\0` character. If longer it will be truncated upon serialization.
   ```csharp
  /// <summary>
  /// Particle name.
  /// </summary>
  [FixedString(16)]
  public string Name;
  ```

- `OptionalAttribute` dictates that the field is not required and thus if not found in the data stream, no error will be thrown. Additionally, this info can (but does not have to be) used to determine how a field should be initialized.
  ```csharp
  /// <summary>
  /// Boxes zones list.
  /// </summary>
  [Optional]
  public List<Box> Boxes;
  ```
  > Note: Value types must also be defined as nullable if made optional using this attribute.
- `ParseNameAttribute` dictates the name to use when serializing. This can be used to overcome C# language reserved words (f.ex. `params`), or to fix typos.
- ...

