using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ThreadTool
{
    //关闭线程
    public static void KillProcess(string strProcessesByName)//关闭线程
    {
        string names = "";
        foreach (Process p in Process.GetProcesses())//GetProcessesByName(strProcessesByName))
        {

            //UnityEngine.Debug.Log(p.ProcessName);
            names += p.ProcessName + "---";
            if (p.ProcessName.Contains(strProcessesByName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(); // possibly with a timeout
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.Message.ToString());
                }
                return;
            }
        }

        TextEditor t = new TextEditor();
        t.text = names;
        t.OnFocus();
        t.Copy();

    }
}
