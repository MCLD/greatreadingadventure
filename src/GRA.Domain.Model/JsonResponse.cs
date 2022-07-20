using System;

namespace GRA.Domain.Model
{
    [Serializable]
    public class JsonResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
