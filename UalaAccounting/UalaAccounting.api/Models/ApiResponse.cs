using System;
public class ApiResponse<T>
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public T Data { get; set; }
    public string TraceId { get; set; }

    public static implicit operator ApiResponse<T>(ApiResponse<byte[]> v)
    {
        throw new NotImplementedException();
    }
}

