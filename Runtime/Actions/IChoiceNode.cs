using System.Collections.Generic;

namespace Actions.Dialogues
{
    public interface IChoiceModel
    {
        string Id { get; }
        int ChoiceType { get; }
        bool IsAvailable();
        ActionContainer Node { get; set; }
    }
    
    public interface IChoiceNode
    {
        List<IChoiceModel> Choices { get; }
    }
}