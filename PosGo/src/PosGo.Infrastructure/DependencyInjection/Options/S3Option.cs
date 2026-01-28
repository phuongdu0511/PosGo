namespace PosGo.Infrastructure.DependencyInjection.Options;

public class S3Option
{
    public string Region { get; set; }
    public string BucketName { get; set; }
    public int ExpireMin { get; set; }
}
