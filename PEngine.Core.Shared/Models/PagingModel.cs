namespace PEngine.Core.Shared.Models
{
  public class PagingModel
  {
    public int Start { get; set; } = 1;
    public int Count { get; set; } = 25;
    public int Total { get; set; } = 0;
    public string SortField { get; set; }
    public bool SortAscending { get; set; } = true;
  }
}