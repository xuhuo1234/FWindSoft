using System.IO;
using System.Text.RegularExpressions;
namespace FWindSoft.Tools
{
    /// <summary>
    /// 文件模块管理
    /// </summary>
    public class FileUnit
    {
        /// <summary>
        /// 获取真实路径
        /// 如果目标地址不存在。则将源地址文件复制到目标路径
        /// 如果源路径不存在，则弹出异常
        /// </summary>
        /// <param name="sorcePath">原始路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns></returns>
        public static string GetRealPath(string sorcePath, string targetPath)
        {
            try
            {
                if (File.Exists(targetPath))
                {
                    return targetPath;
                }
                if (sorcePath != null && File.Exists(sorcePath))
                {
                    File.Copy(sorcePath, targetPath, true);
                    return targetPath;
                }
                else
                {
                    throw new FileNotFoundException(sorcePath);
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 判断指定路径是否存在指定的文件夹名称
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool IsExistFolderName(string path, string folderName)
        {
            string strRe = string.Format(@"(?<=\\){0}(?=\\)", folderName);
            Regex regex = new Regex(strRe);
            return regex.IsMatch(path);
        }
        /// <summary>
        /// 在指定路径中，查找指定文件夹的下一级目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetNextFolderName(string path, string folderName)
        {
            string strRe = string.Format(@"(?<={0}\\)\w+(?=\\)", folderName);
            Regex regex = new Regex(strRe);
            return regex.Match(path).Value;
        }
    }
}
