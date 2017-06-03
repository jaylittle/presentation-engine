using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;

namespace PEngine.Core.Shared
{
  public class ContentHashEntry
  {
    public string FullPath { get; set; }
    public string HashedPath { get; set; }
    public DateTime Modified { get; set; }
  }

  public static class ContentHash
  {
    private static ConcurrentDictionary<string, ContentHashEntry> _hashCache = new ConcurrentDictionary<string, ContentHashEntry>();
    public static ContentHashEntry GetContentHashEntryForFile(string contentRootPath, string wwwRootFolder, string webPath)
    {
      if (!string.IsNullOrEmpty(wwwRootFolder) && !wwwRootFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        wwwRootFolder += Path.DirectorySeparatorChar.ToString();
      }
      ContentHashEntry output = null;
      if (_hashCache.ContainsKey(webPath))
      {
        while (_hashCache.ContainsKey(webPath) && !_hashCache.TryGetValue(webPath, out output));

        //If Cache Entry was found - check file last write time to determine whether or not its valid
        if (output != null && output.Modified != System.IO.File.GetLastWriteTimeUtc(output.FullPath))
        {
          output = null;
          ContentHashEntry removed = null;
          while (_hashCache.ContainsKey(webPath) && !_hashCache.TryRemove(webPath, out removed));
        }
      }
      if (output == null)
      {
        var hashEntry = new ContentHashEntry();

        var oppDirectorySeperatorChar = Path.DirectorySeparatorChar == '/' ? '\\' : '/';

        hashEntry.FullPath = System.IO.Path.Combine(
          contentRootPath,
          wwwRootFolder,
          webPath.Replace(oppDirectorySeperatorChar, Path.DirectorySeparatorChar));

        var md5 = System.Security.Cryptography.MD5.Create();
        using (var reader = System.IO.File.OpenRead(hashEntry.FullPath))
        {
          var md5Bytes = md5.ComputeHash(reader);
          List<string> webPathElements = new List<string>(webPath.Split('.'));
          webPathElements.Insert(webPathElements.Count - 1, Security.BytesToHex(md5Bytes));
          hashEntry.HashedPath = String.Join(".", webPathElements);
        }

        hashEntry.Modified = System.IO.File.GetLastWriteTimeUtc(hashEntry.FullPath);
        while (!_hashCache.ContainsKey(webPath) && !_hashCache.TryAdd(webPath, hashEntry));
      }
      while (_hashCache.ContainsKey(webPath) && !_hashCache.TryGetValue(webPath, out output));
      return output;
    }
  }
}