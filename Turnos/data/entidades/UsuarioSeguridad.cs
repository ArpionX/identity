using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Turnos.data.entidades;

[Table("Usuario_seguridad")]
public partial class UsuarioSeguridad
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("cedula")]
    [StringLength(15)]
    [Unicode(false)]
    public string Cedula { get; set; } = null!;

    [Column("usuario")]
    [StringLength(15)]
    [Unicode(false)]
    public string Usuario { get; set; } = null!;

    [Column("contrasena")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Contrasena { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [Column("estado")]
    public bool Estado { get; set; }

    [Column("idRol")]
    public int IdRol { get; set; }

    [ForeignKey("IdRol")]
    [InverseProperty("UsuarioSeguridads")]
    public virtual RolSeguridad IdRolNavigation { get; set; } = null!;
}
