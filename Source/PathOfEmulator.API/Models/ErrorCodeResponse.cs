namespace PathOfEmulator.API.Models
{
    public class ErrorCodeResponse
    {
        public ErrorCodeMessageResponse Error { get; set; }

        public static ErrorCodeResponse NotFound()
        {
            return new ErrorCodeResponse
            {
                Error = new ErrorCodeMessageResponse
                {
                    Code = 1,
                    Message = "Resource not found"
                }
            };
        }
    }
}
