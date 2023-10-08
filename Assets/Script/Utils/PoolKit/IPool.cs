public interface IPool<T>
{
    /// <summary>
    /// 分配オブジェクト
    /// </summary>
    /// <returns></returns>
    T Allocate();
    /// <summary>
    /// 回収オブジェクト
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool Recycle(T obj);
}
