using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Ejercicio_Adres_FB.Helpers;
using Ejercicio_Adres_FB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;




namespace Ejercicio_Adres_FB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdquisicionesController : ControllerBase
    {
       /* private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/

        private readonly ILogger<AdquisicionesController> _logger;

        public AdquisicionesController(ILogger<AdquisicionesController> logger)
        {
            _logger = logger;
        }

        /* [HttpGet(Name = "GetWeatherForecast")]*/
        [HttpGet("{id}", Name = "GetAdquisicion")]
        [EnableCors("AllowAnyOrigin")]

        public IActionResult Get(Guid id)
        {
            // Lee todas las adquisiciones del archivo JSON.
            var adquisiciones = JsonHelper.ReadJsonFileActive();

            // Intenta encontrar la adquisición especifica por su ID.
            var adquisicion = adquisiciones.FirstOrDefault(a => a.Id == id);

            // Si no se encuentra la adquisicion, retorna un NotFound.
            if (adquisicion == null)
            {
                return NotFound();
            }

            // Si se encuentra la enviamos
            return Ok(adquisicion);
        }

        [HttpGet(Name = "GetAllAdquisiciones")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult GetAll()
        {
            // Lee todas las adquisiciones del archivo JSON.
            var adquisiciones = JsonHelper.ReadJsonFileActive();

            // Retorna la lista completa de adquisiciones.
            return Ok(adquisiciones);
        }

        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Post([FromBody] AdquisicionModel adquisicion)
        {
            // Primero, validamos si el modelo recibido es valido o no
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Luego, si el modelo es válido, crea una nueva instancia de Adquisicion y mapeamos
            var nuevaAdquisicion = new AdquisicionModel
            {
                Id = Guid.NewGuid(), // Genera un nuevo identificador único para esta adquisición.
                Presupuesto = adquisicion.Presupuesto,
                UnidadAdministrativa = adquisicion.UnidadAdministrativa,
                TipoBienServicio = adquisicion.TipoBienServicio,
                Cantidad = adquisicion.Cantidad,
                ValorUnitario = adquisicion.ValorUnitario,
                FechaAdquisicion = adquisicion.FechaAdquisicion,
                Proveedor = adquisicion.Proveedor,
                Documentacion = adquisicion.Documentacion,
                Eliminado = false,
                FechaUltimaActualizacion = DateTime.UtcNow
            };

            // Ahora, lee la lista existente de adquisiciones del archivo JSON, añade la nueva adquisición,
            // y luego escribe la lista actualizada de vuelta al archivo.
            var adquisiciones = JsonHelper.ReadJsonFileAll();
            adquisiciones.Add(nuevaAdquisicion);
            JsonHelper.WriteJsonFile(adquisiciones);

            // Se envia respuesta exitosa con los datos de la adquisicion que se acaba de crear.
            return CreatedAtAction(nameof(Get), new { id = nuevaAdquisicion.Id }, nuevaAdquisicion);
        }


        //Actualizar
        [HttpPut("{id}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Put(Guid id, [FromBody] AdquisicionModel updatedAdquisicion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lee la lista existente de adquisiciones del archivo JSON
            var adquisiciones = JsonHelper.ReadJsonFileActive();

            // Encuentra el índice de la adquisición a actualizar
            var index = adquisiciones.FindIndex(a => a.Id == id);
            if (index == -1)
            {
                return NotFound(); // Si no se encuentra la adquisición, retorna un NotFound
            }

            // Si el modelo es válido y la adquisición existe, actualiza la adquisición
            var adquisicionParaActualizar = adquisiciones[index];
            adquisicionParaActualizar.Presupuesto = updatedAdquisicion.Presupuesto;
            adquisicionParaActualizar.UnidadAdministrativa = updatedAdquisicion.UnidadAdministrativa;
            adquisicionParaActualizar.TipoBienServicio = updatedAdquisicion.TipoBienServicio;
            adquisicionParaActualizar.Cantidad = updatedAdquisicion.Cantidad;
            adquisicionParaActualizar.ValorUnitario = updatedAdquisicion.ValorUnitario;
            adquisicionParaActualizar.FechaAdquisicion = updatedAdquisicion.FechaAdquisicion;
            adquisicionParaActualizar.Proveedor = updatedAdquisicion.Proveedor;
            adquisicionParaActualizar.Documentacion = updatedAdquisicion.Documentacion;
            adquisicionParaActualizar.Eliminado = false;
            adquisicionParaActualizar.FechaUltimaActualizacion = DateTime.UtcNow;

            // Actualiza la lista de adquisiciones en el archivo JSON
            JsonHelper.WriteJsonFile(adquisiciones);

            // Retorna una respuesta exitosa
            return Ok(adquisicionParaActualizar);
        }



        [HttpDelete("{id}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Desactivar(Guid id)
        {

            var adquisiciones = JsonHelper.ReadJsonFileActive();

            // Intenta encontrar la adquisición específica por su ID.
            var adquisicionIndex = adquisiciones.FindIndex(a => a.Id == id);

            // Si no se encuentra la adquisición, retorna un NotFound.
            if (adquisicionIndex == -1)
            {
                return NotFound();
            }

            // Marca la adquisición como eliminada lógicamente en lugar de eliminarla.
            adquisiciones[adquisicionIndex].Eliminado = true;
            adquisiciones[adquisicionIndex].FechaUltimaActualizacion = DateTime.UtcNow;

            // Actualiza la lista de adquisiciones en el archivo JSON con el estado modificado.
            JsonHelper.WriteJsonFile(adquisiciones);

            // Retorna una respuesta indicando que la adquisición fue desactivada exitosamente.
            return Ok();
        }

        [HttpGet("buscar")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar([FromQuery] string fechaDesde, [FromQuery] string fechaHasta, [FromQuery] string proveedor, [FromQuery] string unidadAdministrativa)
        {
            // Lee todas las adquisiciones del archivo JSON.
            var adquisiciones = JsonHelper.ReadJsonFileActive();

            // Aplica los filtros
            if (!string.IsNullOrEmpty(fechaDesde) && DateTime.TryParse(fechaDesde, out DateTime desde))
            {
                adquisiciones = adquisiciones.Where(a => a.FechaAdquisicion >= desde).ToList();
            }

            if (!string.IsNullOrEmpty(fechaHasta) && DateTime.TryParse(fechaHasta, out DateTime hasta))
            {
                adquisiciones = adquisiciones.Where(a => a.FechaAdquisicion <= hasta).ToList();
            }

            if (!string.IsNullOrEmpty(proveedor))
            {
                adquisiciones = adquisiciones.Where(a => a.Proveedor.Contains(proveedor, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(unidadAdministrativa))
            {
                adquisiciones = adquisiciones.Where(a => a.UnidadAdministrativa.Contains(unidadAdministrativa, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Retorna la lista filtrada de adquisiciones
            return Ok(adquisiciones);
        }


        /* public IEnumerable<WeatherForecast> Get()
         {
             return Enumerable.Range(1, 5).Select(index => new WeatherForecast
             {
                 Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                 TemperatureC = Random.Shared.Next(-20, 55),
                 Summary = Summaries[Random.Shared.Next(Summaries.Length)]
             })
             .ToArray();
         }*/













    }
}
