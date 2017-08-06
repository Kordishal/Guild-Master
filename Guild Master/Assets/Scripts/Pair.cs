


public class Pair<T1, T2>
{
    public T1 FirstValue;
    public T2 SecondValue;

    public Pair(T1 firstValue, T2 secondValue)
    {
        FirstValue = firstValue;
        SecondValue = secondValue;
    }


    public override bool Equals(object obj)
    {
        return FirstValue.Equals(obj);
    }
}