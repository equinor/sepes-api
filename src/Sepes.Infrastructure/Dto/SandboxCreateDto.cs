﻿namespace Sepes.Infrastructure.Dto
{
    public class SandboxCreateDto : UpdateableBaseDto
    {
        public string Name { get; set; }    

        public string Region { get; set; }

        public string UseTemplate { get; set; }    
    }
}
