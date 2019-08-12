﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoDatabaseService.Contracts
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Nev { get; set; }
        public string SharePointId { get; set; }
        public int ContactDetailId { get; set; }
        public string WLSzam { get; set; }
        public string Adoszam { get; set; }
        public string KSHSzam { get; set; }
        public string Cegjegyzekszam { get; set; }
        public CurrencyDto KonyvelesPenzneme { get; set; }
        public CurrencyDto BeszamoloPenzneme { get; set; }

        public List<AddressDto> Address { get; set; }
   
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
    }
}
