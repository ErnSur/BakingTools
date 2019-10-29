namespace QuickEye.BakingTools
{
    public class ToglabbleParameter<T>
    {
        public bool isOn;
        public T value;

        public static implicit operator T(ToglabbleParameter<T> p) => p.value;
    }
}
