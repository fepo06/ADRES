const API_URL = 'https://localhost:7123/Adquisiciones';

let isEditing = false; // Indica si el formulario se usa para editar
let editingId = null; // Almacena el ID de la adquisición que se está editando

const modal = document.getElementById('form-modal');
const btnAddAdquisicion = document.getElementById('show-form-btn');
const btnCloseForm = document.getElementById('close-form-btn');

document.addEventListener('DOMContentLoaded', function () {

    btnAddAdquisicion.onclick = function() {
        form.reset();
        isEditing = false;
        editingId = null;
        modal.style.display = "block";
    }

    btnCloseForm.addEventListener('click', function() {
        modal.style.display = 'none';
    });

    window.onclick = function(event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

    const form = document.getElementById('adquisiciones-form');
    
    form.addEventListener('submit', function (e) {
        e.preventDefault(); 

        // Recolecta los datos del formulario 
        const formData = {
            Presupuesto: document.getElementById('budget').value,
            UnidadAdministrativa: document.getElementById('administrativeUnit').value,
            TipoBienServicio: document.getElementById('itemType').value,
            Cantidad: document.getElementById('quantity').value,
            ValorUnitario: document.getElementById('unitValue').value,
            FechaAdquisicion: document.getElementById('acquisitionDate').value,
            Proveedor: document.getElementById('supplier').value,
            Documentacion: document.getElementById('documentation').value
        };

        const fetchOptions = {
            method: isEditing ? 'PUT' : 'POST', // Cambia el método dependiendo del estado
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(formData)
        };

        // Utiliza la URL adecuada dependiendo de si estás creando o editando
        const fetchURL = isEditing ? `${API_URL}/${editingId}` : API_URL;

        // Envía la petición a la ADRES_API
        fetch(fetchURL, fetchOptions)
        .then(response => {
            console.log('Error:', response);
            if (!response.ok) {
                return response.json().then(err => {throw err;});
            }
            return response.json();
        })
        .then(data => {
            console.log('Success:', data);
            alert(`Adquisición ${isEditing ? 'actualizada' : 'registrada'} con éxito!`);
            form.reset();
            isEditing = false; // Restablece el estado de edición
            cargarAdquisiciones(); // Recargar las adquisiciones
            modal.style.display = 'none'; // Ocultar el modal
        })
        .catch((error) => {
            console.error('Error:', error);
            // Si hubo error muestro las validaciones
            let errorMessage = 'Ocurrió un error al registrar la adquisición.';
            if (error.errors) {
                errorMessage += '\n\nErrores de validación:';
                for (const field in error.errors) {
                    errorMessage += `\n- ${field}: ${error.errors[field].join(', ')}`;
                }
            }
            alert(errorMessage);
        });
    });

    function cargarAdquisiciones() {
        fetch(API_URL)
        .then(response => response.json())
        .then(adquisiciones => {

            actualizarTabla(adquisiciones);
        })
        .catch(error => {
            console.error('Error al cargar las adquisiciones:', error);
        });
    }

    cargarAdquisiciones();

});

function editarAdquisicion(id) {
    fetch(`${API_URL}/${id}`)
    .then(response => response.json())
    .then(adquisicion => {
        // Se llena el formulario con los datos de la adquisición
        document.getElementById('budget').value = adquisicion.presupuesto;
        document.getElementById('administrativeUnit').value = adquisicion.unidadAdministrativa;
        document.getElementById('itemType').value = adquisicion.tipoBienServicio;
        document.getElementById('quantity').value = adquisicion.cantidad;
        document.getElementById('unitValue').value = adquisicion.valorUnitario;
        document.getElementById('acquisitionDate').value = formatearFecha(adquisicion.fechaAdquisicion);
        document.getElementById('supplier').value = adquisicion.proveedor;
        document.getElementById('documentation').value = adquisicion.documentacion;

        // Muestra el modal del formulario
        console.log("Modal", modal);
        modal.style.display = "block";

        isEditing = true;
        editingId = id;

    })
    .catch(error => {
        console.error('Error al cargar la adquisición:', error);
    });
}

function desactivarAdquisicion(id) {
    const confirmacion = confirm("¿Estás seguro de que deseas desactivar esta adquisición?");
    if (confirmacion) {
        fetch(`${API_URL}/${id}`, {
            method: 'DELETE',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({ IsActive: false })
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('No se pudo desactivar la adquisición');
            }
            alert('Adquisición desactivada con éxito');
            filtrarAdquisiciones(); // Recargar la lista para reflejar el cambio
        })
        .catch(error => console.error('Error:', error));
    }
}

function actualizarTabla(adquisiciones) {
    const tableBody = document.getElementById('acquisitions-list');
    // Limpia el cuerpo de la tabla
    tableBody.innerHTML = '';
        
    // Agregar encabezados si no existen
    let thead = tableBody.createTHead();
    let row = thead.insertRow();
    let headers = ['Presupuesto', 'Unidad Administrativa', 'Tipo de Bien o Servicio', 'Cantidad', 'Valor Unitario', 'Fecha Adquisición', 'Proveedor', 'Acciones'];
    headers.forEach(headerText => {
        let header = document.createElement('th');
        let textNode = document.createTextNode(headerText);
        header.appendChild(textNode);
        row.appendChild(header);
    });

    let tbody = document.createElement('tbody');
    adquisiciones.forEach(adquisicion => {
        let row = tbody.insertRow();
        row.insertCell().textContent = formatearMoneda(adquisicion.presupuesto);
        row.insertCell().textContent = adquisicion.unidadAdministrativa;
        row.insertCell().textContent = adquisicion.tipoBienServicio;
        row.insertCell().textContent = formatearMoneda(adquisicion.cantidad);
        row.insertCell().textContent = formatearMoneda(adquisicion.valorUnitario);
        row.insertCell().textContent = formatearFecha(adquisicion.fechaAdquisicion);
        row.insertCell().textContent = adquisicion.proveedor;

        // Agregar los botones de editar y desactivar
        let editCell = row.insertCell();
        let editButton = document.createElement('button');
        editButton.textContent = 'Editar';
        editButton.setAttribute('data-id', adquisicion.id);
        editButton.onclick = function() {
            editarAdquisicion(this.getAttribute('data-id'));
        };
        editCell.appendChild(editButton);

        let deactivateButton = document.createElement('button');
        deactivateButton.textContent = 'Desactivar';
        deactivateButton.setAttribute('data-id', adquisicion.id);
        deactivateButton.onclick = function() {
            desactivarAdquisicion(this.getAttribute('data-id'));
        };
        editCell.appendChild(deactivateButton);
    });
    tableBody.appendChild(tbody);
}

function filtrarAdquisiciones() {
    const fechaDesde = document.getElementById('fecha-desde').value;
    const fechaHasta = document.getElementById('fecha-hasta').value;
    const proveedor = document.getElementById('proveedor').value;
    const unidadAdministrativa = document.getElementById('unidad-administrativa').value;

    let query = `${API_URL}/buscar?`;
    if (fechaDesde) query += `fechaDesde=${fechaDesde}&`;
    if (fechaHasta) query += `fechaHasta=${fechaHasta}&`;
    if (proveedor) query += `proveedor=${encodeURIComponent(proveedor)}&`;
    if (unidadAdministrativa) query += `unidadAdministrativa=${encodeURIComponent(unidadAdministrativa)}`;

    fetch(query)
    .then(response => response.json())
    .then(adquisiciones => {
        // Actualiza la tabla con las adquisiciones filtradas
        actualizarTabla(adquisiciones);
    })
    .catch(error => {
        console.error('Error al filtrar las adquisiciones:', error);
    });
}

function formatearFecha(fechaConHora) {
    const fecha = new Date(fechaConHora);
    const year = fecha.getFullYear();
    const month = (fecha.getMonth() + 1).toString().padStart(2, '0'); 
    const day = fecha.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`; // Retorna la fecha en formato 'YYYY-MM-DD'
}

function formatearMoneda(numero) {
    return new Intl.NumberFormat('es-CO', { style: 'decimal', minimumFractionDigits: 0 }).format(numero) ;
}
