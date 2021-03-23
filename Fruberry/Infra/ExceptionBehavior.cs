namespace Fruberry {
    public enum ExceptionBehavior {
        Default = 0,
        Abort = 1, //act as much as possible as if the throwing method had not been called
        Throw = 2, //throw an exception
        BestEffort = 4 //attempt to perform some valid action
    }
}
