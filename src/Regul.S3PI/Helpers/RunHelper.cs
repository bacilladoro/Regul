using System;
using System.Collections.Generic;
using System.IO;
using Regul.S3PI.Interfaces;

namespace Regul.S3PI.Helpers
{
    public interface IRunHelper
    {
        byte[] Result { get; }
    }
    public static class RunHelper
    {
        //public static int Run(Type mainForm, params string[] args)
        //{
        //    bool useClipboard = false;
        //    bool useFile = false;
        //    List<string> files = new List<string>();

        //    List<char> switchChars = new List<char>(new char[] { '/', '-', });
        //    switchChars.Remove(Path.DirectorySeparatorChar);
        //    foreach (string s in args)
        //    {
        //        string p = s;
        //        if (p.Length > 1 && switchChars.Contains(p[0]))
        //        {
        //            if ("clipboard".StartsWith(p.Substring(1).ToLower()))
        //                useClipboard = true;
        //            else
        //            {
        //                return 1;
        //            }
        //        }
        //        else
        //            files.Add(s);
        //    }

        //    if (useClipboard && files.Count > 0)
        //    {
        //        return 2;
        //    }
        //    if (files.Count > 1)
        //    {
        //        return 3;
        //    }

        //    useFile = files.Count > 0;
        //    useClipboard = !useFile;

        //    Stream ms;

        //    if (useClipboard)
        //    {
        //        ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
        //        if (ms == null)
        //        {
        //            CopyableMessageBox.Show("Invalid clipboard content",
        //                "Fail", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Stop);
        //            return 4;
        //        }
        //        Clipboard.Clear();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            ms = File.Open(files[0], FileMode.Open, FileAccess.ReadWrite);
        //        }
        //        catch (Exception ex)
        //        {
        //            CopyableMessageBox.IssueException(ex, files[0] + "\n" + mainForm.Assembly.FullName, "Failed to open file");
        //            return -1;
        //        }
        //    }


        //    byte[] result = null;
        //    try
        //    {
        //        Form theForm = (Form)mainForm.GetConstructor(new Type[] { typeof(Stream), }).Invoke(new object[] { ms, });
        //        Environment.ExitCode = 1;
        //        Application.Run(theForm);
        //        if (Environment.ExitCode != 0)
        //            return 0;

        //        result = ((IRunHelper)theForm).Result;
        //        if (result == null)
        //            return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return -1;
        //    }

        //    if (useClipboard)
        //    {
        //        Clipboard.SetData(DataFormats.Serializable, new MemoryStream(result));
        //    }
        //    else
        //    {
        //        ms.Position = 0;
        //        ms.SetLength(0);
        //        ms.Write(result, 0, result.Length);
        //    }

        //    return 0;
        //}
    }
}
