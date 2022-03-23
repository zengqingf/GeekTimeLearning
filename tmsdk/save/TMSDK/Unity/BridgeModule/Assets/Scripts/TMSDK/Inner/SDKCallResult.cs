
namespace TMSDKClient
{
    public class SDKCallResult<T>
    {
        public static readonly int RESULT_SUCCESS = 0;
        public static readonly int RESULT_EXCEPTION = -1;

        public int code = 0;
        public string message;
        public T obj;
    }
}
