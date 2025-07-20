namespace Online_Learning.Models.DTOs.Response.Common
{
    public class ServiceResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public ServiceResponse() { }

        //public ServiceResponse(int StatusCode = 400, me)
        //{
            
        //}
    }
}
