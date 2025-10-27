using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Turnos.data.entidades;

[Table("Rol_seguridad")]
public partial class RolSeguridad
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Nombre { get; set; }

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<UsuarioSeguridad> UsuarioSeguridads { get; set; } = new List<UsuarioSeguridad>();
}
