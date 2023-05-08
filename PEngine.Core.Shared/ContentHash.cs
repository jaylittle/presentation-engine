using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PEngine.Core.Shared
{
  public class ContentHashEntry
  {
    public string FullPath { get; set; }
    public string WebPath { get; set; }
    public string Hash { get; set; }
    public DateTime Modified { get; set; }
    public bool Transformable
    {
      get
      {
        return !string.IsNullOrEmpty(FullPath)
          && FullPath.EndsWith(".css", StringComparison.OrdinalIgnoreCase);
      }
    }
    public byte[] Transformation { get; set; } = null;
  }

  public static class ContentHash
  {
    private static ConcurrentDictionary<string, ContentHashEntry> _hashCache = new ConcurrentDictionary<string, ContentHashEntry>();
    public static async Task<ContentHashEntry> GetContentHashEntryForFile(string contentRootPath, string[] wwwRootFolders
      , string webPath, Func<string, string, string> GetHashedUrl = null)
    {
      return await Task.Run<ContentHashEntry>(() =>
      {
        ContentHashEntry output = null;
        foreach (var wwwRootFolderRaw in wwwRootFolders)
        {
          var wwwRootFolder = wwwRootFolderRaw;
          if (!string.IsNullOrEmpty(wwwRootFolder) && !wwwRootFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
          {
            wwwRootFolder += Path.DirectorySeparatorChar.ToString();
          }
          var actualFileRoot = System.IO.Path.Combine(contentRootPath, wwwRootFolder);
          if (_hashCache.ContainsKey(webPath))
          {
            while (_hashCache.ContainsKey(webPath) && !_hashCache.TryGetValue(webPath, out output));

            //If Cache Entry was found - check file last write time to determine whether or not its valid
            if (output != null  && System.IO.File.Exists(output.FullPath)
                && output.Modified != System.IO.File.GetLastWriteTimeUtc(output.FullPath))
            {
              output = null;
              ContentHashEntry removed = null;
              while (_hashCache.ContainsKey(webPath) && !_hashCache.TryRemove(webPath, out removed));
            }
          }
          if (output == null)
          {
            var hashEntry = new ContentHashEntry() {
              WebPath = webPath
            };

            var oppDirectorySeperatorChar = Path.DirectorySeparatorChar == '/' ? '\\' : '/';

            hashEntry.FullPath = System.IO.Path.Combine(
              actualFileRoot,
              webPath).Replace(oppDirectorySeperatorChar, Path.DirectorySeparatorChar);

            if (System.IO.File.Exists(hashEntry.FullPath))
            {
              // Make sure target file's full path lives within the location we want to restrict users to
              var fileInfo = new System.IO.FileInfo(hashEntry.FullPath);
              if (fileInfo.FullName.StartsWith(actualFileRoot, StringComparison.OrdinalIgnoreCase))
              {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var reader = System.IO.File.Open(hashEntry.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                  var md5Bytes = md5.ComputeHash(reader);
                  hashEntry.Hash = Security.BytesToHex(md5Bytes);
                }
                hashEntry.Modified = System.IO.File.GetLastWriteTimeUtc(hashEntry.FullPath);

                //Check for transformable content
                if (hashEntry.Transformable)
                {
                  if (!string.IsNullOrEmpty(hashEntry.FullPath) && hashEntry.FullPath.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                  {
                    hashEntry.Transformation = System.Text.Encoding.UTF8.GetBytes(
                      TransformCSS(contentRootPath, wwwRootFolders, webPath, System.IO.File.ReadAllText(hashEntry.FullPath), GetHashedUrl)
                    );
                  }
                }

                while (!_hashCache.ContainsKey(webPath) && !_hashCache.TryAdd(webPath, hashEntry));
              }
              else
              {
                throw new Exception("Requested File Path exists outside the specified root location!");
              }
            }
          }
          while (_hashCache.ContainsKey(webPath) && !_hashCache.TryGetValue(webPath, out output));
          if (output != null)
          {
            break;
          }
        }
        return output;
      });
    }

    //For reference here is the Unescaped Regex string (in case you want to test this in regexr or something): url\("*'*(.[^("'\(\)]+)"*'*\)
    private static Regex CSS_URL_REGEX = new Regex("url\\(\"*'*(.[^(\"'\\(\\)]+)\"*'*\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant
      | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private static string TransformCSS(string contentRootPath, string[] wwwRootFolders, string webPath, string originalCss, Func<string, string, string> GetHashedUrl = null)
    {
      var originalAbsoluteDirectory = System.IO.Path.GetDirectoryName(
        System.IO.Path.Combine(
          "/",
          webPath
        ).Replace('\\', '/')
      );
      if (!originalAbsoluteDirectory.EndsWith('/'))
      {
        originalAbsoluteDirectory += "/";
      }

      return CSS_URL_REGEX.Replace(originalCss, (m) => {
        var fullMatch = m.Groups[0].Value;
        var matchedUrl = m.Groups[m.Groups.Count - 1].Value;
        var returnUrl = matchedUrl;

        if (!Helpers.IsUrlAbsolute(matchedUrl))
        {
          var matchedAbsoluteUrl = Path.GetFullPath(
            System.IO.Path.Combine(
              originalAbsoluteDirectory,
              matchedUrl).Replace('\\', '/'
            )
          );
          if (matchedAbsoluteUrl.StartsWith(Path.DirectorySeparatorChar))
          {
            matchedAbsoluteUrl = matchedAbsoluteUrl.TrimStart(Path.DirectorySeparatorChar);
          }
          
          var matchedHashEntry = GetContentHashEntryForFile(contentRootPath, wwwRootFolders, matchedAbsoluteUrl, GetHashedUrl).Result;
          if (matchedHashEntry != null)
          {
            returnUrl = GetHashedUrl(matchedHashEntry.Hash, matchedHashEntry.WebPath);
          }
        }

        return fullMatch.Replace(matchedUrl, returnUrl);
      });
    }
  }
}