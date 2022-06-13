﻿using System;

namespace Application.ViewModels.Customer
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }
        
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }
    }
}