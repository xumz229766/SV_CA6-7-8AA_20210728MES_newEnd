using System.Toolkit.Interfaces;
namespace Motion.Interfaces
{
    /// <summary>
    ///     ��ʾһ����Ӧ����
    /// </summary>
    public interface IResponser<T> : IAutomatic where T : struct
    {
        bool Value { set; }
    }
}