﻿using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace Apeek.ViewModels.Models.Impl{
    public class MostRentedItems    {        public int Id { get; set; }        public string ItemName { get; set; }        public int Count { get; set; }        public DateTime StartDate { get; set; }        public DateTime Enddate { get; set; }        public int Status { get; set; }        public int UserId { get; set; }        public double Total { get; set; }        public DateTime Date { get; set; }        public string Month { get; set; }        public int Year { get; set; }        public List<MostRentedItems> mostRentedItems { get; set; }        public List<MostRentedItems> totalLoanedItems { get; set; }        public List<MostRankingCategory> topRankingofCategory { get; set; }    }    public class MostRankingCategory    {        public int Id { get; set; }        public string CategoryName { get; set; }        public int Count { get; set; }    }}