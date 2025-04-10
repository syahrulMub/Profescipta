$(document).ready(function () {
  //   Initialize DataTable
  const table = $("#salesOrderTable").DataTable({
    ajax: {
      url: "/SalesOrder/GetSalesOrder",
      type: "GET",
      data: function (d) {
        d.search = $("#keyword").val();
        const rawDate = $("#orderDate").val();
        if (rawDate) {
          const dateObj = new Date(rawDate);
          d.dateTime = dateObj.toISOString();
        } else {
          d.dateTime = null;
        }
      },
      dataSrc: "",
    },
    columns: [
      {
        data: null,
        title: "Actions",
        render: function (data, type, row) {
          return `<div class="col-12 gap-2 d-flex justify-content-center">
                          <button class="btn btn-primary btn-sm" onclick="editOrder(${row.soOrderId})"><i class="fa-solid fa-pen"></i></button>
                          <button class="btn btn-danger btn-sm" onclick="deleteOrder(${row.soOrderId})"><i class="fa-solid fa-trash"></i></button>
                      </div>`;
        },
        className: "no-export",
      },
      { data: "soOrderId", visible: false },
      { data: "orderNo", title: "Sales Order" },
      { data: "orderDate", title: "Order Date" },
      { data: "customerName", title: "Customer Name" },
    ],
    responsive: true,
    paging: true,
    searching: true,
    ordering: true,
    dom: "Bfrtip",
    buttons: [
      {
        extend: "excelHtml5",
        title: "SalesOrders",
        exportOptions: {
          columns: ":visible:not(.no-export)", // jangan export kolom aksi
        },
        className: "d-none", // sembunyikan tombol bawaan, kita trigger manual
      },
    ],
  });

  // Handle Search Button Click
  $("#searchButton").on("click", function () {
    table.ajax.reload(); 
  });

  // handle export excel from datatable
  $("#exportExcel").on("click", function () {
    table.button(".buttons-excel").trigger();
  });
});

$("#addNewData").on("click", function () {
  window.location.href = "/SalesOrder/Create";
});

// Function to handle edit action
function editOrder(orderId) {
  window.location.href = "/SalesOrder/Edit/" + orderId;
}

// Function to handle delete action
function deleteOrder(orderId) {
  Swal.fire({
    title: "Are you sure?",
    text: "delete this Sales Order",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        url: "/SalesOrder/Delete/" + orderId,
        type: "DELETE",
        success: function (response) {
          // Reload the DataTable after deletion
          $("#salesOrderTable").DataTable().ajax.reload();
          Swal.fire("Deleted!", "Your file has been deleted.", "success");
        },
        error: function (xhr, status, error) {
          console.error("Error deleting order:", error);
          Swal.fire(
            "Error!",
            "There was an error deleting the order.",
            "error"
          );
        },
      });
    }
  });
}
