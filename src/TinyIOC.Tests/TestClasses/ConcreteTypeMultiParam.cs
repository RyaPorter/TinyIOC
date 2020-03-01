namespace TinyIOC.Tests.TestClasses
{
    public class ConcreteTypeMultiParam
    {

        FirstParam _f;
        SecondParam _s;
        IInterfaceParam _i;

        public ConcreteTypeMultiParam(FirstParam param1, SecondParam param2 /*, IInterfaceParam iParam*/){
            _f = param1;
            _s = param2;
            //_i = iParam;
        }   
    }

    public class FirstParam {

    }

    public class SecondParam {

    }

    public interface IInterfaceParam {

    }

    public class InterfaceParam : IInterfaceParam{

    }
}