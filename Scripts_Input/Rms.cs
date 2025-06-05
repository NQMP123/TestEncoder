using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class Rms
{
	public static int status;

	public static sbyte[] data;

	public static string filename;

	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public static void saveRms(string filename, sbyte[] data)
	{
		if (Thread.CurrentThread.Name == Main.mainThreadName)
		{
			__saveRms(filename, data);
		}
		else
		{
			_saveRms(filename, data);
		}
	}

	public static sbyte[] loadRms(string filename)
	{
		if (Thread.CurrentThread.Name == Main.mainThreadName)
		{
			return __loadRms(filename);
		}
		return _loadRms(filename);
	}

	public static string loadRmsString(string fileName)
	{
		sbyte[] array = loadRms(fileName);
		if (array == null)
		{
			return null;
		}
		DataInputStream dataInputStream = new DataInputStream(array);
		try
		{
			string result = dataInputStream.readUTF();
			dataInputStream.close();
			return result;
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
		return null;
	}

	public static byte[] convertSbyteToByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
	}

	public static void saveRmsString(string filename, string data)
	{
		DataOutputStream dataOutputStream = new DataOutputStream();
		try
		{
			dataOutputStream.writeUTF(data);
			saveRms(filename, dataOutputStream.toByteArray());
			dataOutputStream.close();
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
	}

	private static void _saveRms(string filename, sbyte[] data)
	{
		if (status != 0)
		{
			Debug.LogError("Cannot save Rms " + filename + " because current is saving " + Rms.filename);
			return;
		}
		Rms.filename = filename;
		Rms.data = data;
		status = 2;
		int i;
		for (i = 0; i < 500; i++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (i == 500)
		{
			Debug.LogError("TOO LONG TO SAVE Rms " + filename);
		}
	}

	private static sbyte[] _loadRms(string filename)
	{
		if (status != 0)
		{
			Debug.LogError("Cannot load Rms " + filename + " because current is loading " + Rms.filename);
			return null;
		}
		Rms.filename = filename;
		data = null;
		status = 3;
		int i;
		for (i = 0; i < 500; i++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (i == 500)
		{
			Debug.LogError("TOO LONG TO LOAD Rms " + filename);
		}
		return data;
	}

	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			__saveRms(filename, data);
			status = 0;
		}
		else if (status == 3)
		{
			status = 1;
			data = __loadRms(filename);
			status = 0;
		}
	}

	public static int loadRmsInt(string file)
	{
		sbyte[] array = loadRms(file);
		return (array != null) ? array[0] : (-1);
	}

	public static void saveRmsInt(string file, int x)
	{
		try
		{
			saveRms(file, new sbyte[1] { (sbyte)x });
		}
		catch (Exception)
		{
		}
	}

	public static string GetiPhoneDocumentsPath()
	{
		return Application.persistentDataPath;
	}
	private static void __saveRms(string filename, sbyte[] data)
	{
		string text = GetiPhoneDocumentsPath() + "/" + filename;
		FileStream fileStream = new FileStream(text, FileMode.Create);
		fileStream.Write(ArrayCast.cast(data), 0, data.Length);
		fileStream.Flush();
		fileStream.Close();
		Main.setBackupIcloud(text);
	}

	private static sbyte[] __loadRms(string filename)
	{
		try
		{
			FileStream fileStream = new FileStream(GetiPhoneDocumentsPath() + "/" + filename, FileMode.Open);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			sbyte[] array2 = ArrayCast.cast(array);
			return ArrayCast.cast(array);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static void clearAll()
	{
		Cout.LogError3("clean Rms");
		FileInfo[] files = new DirectoryInfo(GetiPhoneDocumentsPath() + "/").GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			try
			{
                fileInfo.Delete();
            }
			catch (Exception)
			{
			}
	
		}
	}

	public static void DeleteStorage(string path)
	{
		try
		{
			File.Delete(GetiPhoneDocumentsPath() + "/" + path);
		}
		catch (Exception)
		{
		}
	}

	public static string ByteArrayToString(byte[] ba)
	{
		string text = BitConverter.ToString(ba);
		return text.Replace("-", string.Empty);
	}

	public static byte[] StringToByteArray(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public static void deleteRecord(string name)
	{
		try
		{
			PlayerPrefs.DeleteKey(name);
		}
		catch (Exception ex)
		{
			Cout.println("loi xoa Rms --------------------------" + ex.ToString());
		}
	}

	public static void clearRms()
	{
		deleteRecord("data");
		deleteRecord("dataVersion");
		deleteRecord("map");
		deleteRecord("mapVersion");
		deleteRecord("skill");
		deleteRecord("killVersion");
		deleteRecord("item");
		deleteRecord("itemVersion");
	}

	public static void saveIP(string strID)
	{
		saveRmsString("NRIPlink", strID);
	}

	public static string loadIP()
	{
		string text = loadRmsString("NRIPlink");
		if (text == null)
		{
			return null;
		}
		return text;
	}
}
