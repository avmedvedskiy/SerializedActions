using System.Collections.Generic;

namespace Actions.Dialogues
{
    public interface IChoiceModel
    {
        bool IsAvailable();
        ActionContainer Node { get; set; }
    }
    
    public interface IChoiceNode
    {
        List<IChoiceModel> Choices { get; }
    }
}