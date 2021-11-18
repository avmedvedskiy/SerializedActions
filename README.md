 [![Releases](https://img.shields.io/github/release/avmedvedskiy/SerializedActions.svg)](https://github.com/avmedvedskiy/SerializedActions/releases)

# Serialized Actions
Actions based on the `ManagedReference` attribute and `IAction` interface allow you to run a sequence of a custom actions.

### Used UniTask framework for async operations

![image](https://user-images.githubusercontent.com/17832838/142039615-e25db621-9360-4155-a66c-afffa5546291.png)


#### Features
 - Custom actions with async Run method
 - Custom ActionRunner support 
 - Support Nested Sequences

#### Install via git URL

`https://github.com/avmedvedskiy/SerializedActions.git?path=Assets/SerializedActions`

from the `Add package from git URL` option.

## 🔰 Usage

```cs
  public List<IAction> actions;
  async void RunAsync()
  {
      ActionRunner runner = new ActionRunner();
      await runner.RunAsync(actions);
      Debug.Log("Finish");
  }
```
