
namespace com.afjk.LocalDeviceFinder
{
    public interface IReceiveDataFactory
    {
        IReceiveData Create();
    }

    public interface IReceiveData
    {
        byte[] Serialize();

        IReceiveData Deserialize(byte[] bytes);
    }
}