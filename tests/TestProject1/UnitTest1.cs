namespace TestProject1;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        string path1 = "../test.proj";
        string path2 = "test/test";

        string path3 = Path.GetDirectoryName(path1) + Path.DirectorySeparatorChar + path2;
        
        Assert.True(true);
    }
}