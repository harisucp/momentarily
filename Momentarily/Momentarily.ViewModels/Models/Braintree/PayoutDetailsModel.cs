﻿using System.ComponentModel.DataAnnotations;
        //public int Id { get; set; }
        [Required(ErrorMessage = "UserId Required")]
        //[Required(ErrorMessage = "AccountNumber Required")]
        public string AccountNumber { get; set; }
        //[Required(ErrorMessage = "RoutingNumber Required")]
        public string RoutingNumber { get; set; }
        //[MaxLength(10, ErrorMessage = "Maximum 10 digits allowed")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }