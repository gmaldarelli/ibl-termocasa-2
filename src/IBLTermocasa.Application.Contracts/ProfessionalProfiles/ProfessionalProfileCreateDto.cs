        using System;
        using System.ComponentModel.DataAnnotations;
        using System.Collections.Generic;

        namespace IBLTermocasa.ProfessionalProfiles
        {
            public class ProfessionalProfileCreateDto
            {
                [Required]
                public string Code { get; set; } = null!;
                [Required]
                public string Name { get; set; } = null!;
                public double StandardPrice { get; set; }
            }
        }