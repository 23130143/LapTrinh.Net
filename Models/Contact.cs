using System;
using System.Collections.Generic;

namespace Bookstore.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string? BusinessId { get; set; }

    public string? BusinessName { get; set; }

    public string? TaxCode { get; set; }
}
