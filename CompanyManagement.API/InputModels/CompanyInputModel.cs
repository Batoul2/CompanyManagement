using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.API.InputModels
{
  public class CompanyInputModel
{
  [Required]
  [Length(1,50)]
    public string Name { get; set; } = string.Empty;
}

}