﻿using DevExpress.Data.Filtering;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizaProdaje.Domain.Helpers
{
    public class CriteriaFilterHelper
    {
        public GridViewDataColumn Column { get; set; }
        public CriteriaOperator Criteria { get; set; }
        public string Value { get; set; }
    }
}