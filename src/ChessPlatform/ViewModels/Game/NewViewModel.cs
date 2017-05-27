using System.ComponentModel.DataAnnotations;

namespace ChessPlatform.ViewModels.Game
{
    public class NewViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Room Password")]
        public string Password { get; set; }

        [Display(Name = "Allow spectate")]
        public bool AllowSpectate { get; set; }

        [Display(Name = "Minimum number of points")]
        [Range(0, int.MaxValue)]
        public int? MinimumNumberOfPoints { get; set; }

        [Display(Name = "Maximum number of points")]
        [Range(0, int.MaxValue)]
        public int? MaximumNumberOfPoints { get; set; }

        [Display(Name = "Play with computer")]
        public bool WithComputer { get; set; }
    }
}
