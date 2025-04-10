$(document).ready(function () {
  $.ajax({
    url: "/ComCustomer/GetComCustomer",
    type: "GET",
    success: function (data) {
      populateCustomerDropdown(data);
    },
    error: function () {
      console.error("failed to fetch data");
    },
  });

  const itemTable = $("#itemDetailsTable1").DataTable({
    columns: [
      { data: "itemName", title: "Item Name" },
      { data: "quantity", title: "Quantity" },
      {
        data: "price",
        title: "Price",
        render: function (data, type, row) {
          return typeof data === "number" ? formatCurrency(data) : data;
        },
      },
      {
        data: "total",
        title: "Total",
        render: function (data, type, row) {
          return typeof data === "number" ? formatCurrency(data) : data;
        },
      },
      {
        data: null,
        title: "Actions",
        render: function (data, type, row) {
          return row.isNew || row.isEditing
            ? `<button class="btn btn-success btn-sm save-item">Save</button>
               <button class="btn btn-danger btn-sm cancel-item">Cancel</button>`
            : `<button class="btn btn-primary btn-sm edit-item">Edit</button>
               <button class="btn btn-danger btn-sm delete-item">Delete</button>`;
        },
      },
    ],
    footerCallback: function (row, data, start, end, display) {
      // Hitung total amount
      let totalAmount = 0;
      data.forEach(function (row) {
        if (typeof row.total === "number") {
          totalAmount += row.total;
        }
      });

      // Total items = jumlah baris
      let totalItems = 0;
      data.forEach(function (row) {
        if (typeof row.quantity === "number") {
          totalItems += row.quantity;
        }
      });

      // Update footer
      $("#footer-total-items").html(totalItems);
      $("#footer-total-amount").html(formatCurrency(totalAmount));
    },
  });

  // Add Item Button Click
  $("#addItem").on("click", function () {
    // Add a new row with input fields
    itemTable.row
      .add({
        itemName: `<input type="text" class="form-control item-name" placeholder="Enter Item Name">`,
        quantity: `<input type="number" class="form-control item-quantity" placeholder="Enter Quantity">`,
        price: `<input type="number" class="form-control item-price" placeholder="Enter Price">`,
        total: `<span class="item-total">0</span>`,
        isNew: true, // Mark this row as new
      })
      .draw(false);
  });

  // Save Item (new or edited)
  $("#itemDetailsTable1").on("click", ".save-item", function () {
    const rowEl = $(this).closest("tr");
    const row = itemTable.row(rowEl);
    const rowData = row.data();

    const itemName = rowEl.find(".item-name").val();
    const quantity = parseInt(rowEl.find(".item-quantity").val(), 10);
    const price = parseFloat(rowEl.find(".item-price").val(), 10);

    if (itemName && quantity > 0 && price > 0) {
      const total = quantity * price;

      const updatedData = {
        soItemId: rowData.soItemId || null,
        soOrderId: rowData.soOrderId || null,
        itemName,
        quantity,
        price,
        total,
        isNew: false,
        isEditing: false,
      };

      row.data(updatedData).draw(false);
    } else {
      alert("Invalid input. Please fill out all fields with valid values.");
    }
  });

  // Cancel Add or Edit
  $("#itemDetailsTable1").on("click", ".cancel-item, .cancel-edit", function () {
    const rowEl = $(this).closest("tr");
    const row = itemTable.row(rowEl);
    const data = row.data();

    // Jika baris baru → hapus
    if (data.isNew) {
      row.remove().draw();
    } else if (data.originalData) {
      // Jika sedang edit → restore dari originalData
      const restoredData = { ...data.originalData, isEditing: false };
      row.data(restoredData).draw(false);
    }
  });

  // Edit Item
  $("#itemDetailsTable1").on("click", ".edit-item", function () {
    const rowEl = $(this).closest("tr");
    const row = itemTable.row(rowEl);
    const data = row.data();

    // Simpan original data agar bisa di-cancel
    const originalData = { ...data };

    // Replace cell content with input fields
    const editedData = {
      ...data,
      itemName: `<input type="text" class="form-control item-name" value="${data.itemName}">`,
      quantity: `<input type="number" class="form-control item-quantity" value="${data.quantity}">`,
      price: `<input type="number" class="form-control item-price" value="${data.price}">`,
      total: `<span class="item-total">${data.total}</span>`,
      isEditing: true,
      originalData: originalData,
    };

    row.data(editedData).draw(false);
  });

  $("#itemDetailsTable1").on("click", ".delete-item", function () {
    const row = $(this).closest("tr");
    itemTable.row(row).remove().draw();
  });

  // Handle Submit Form
  $("#create-order").on("click", function () {
    // Collect header data
    const salesOrderHeader = {
      orderNo: $("#salesOrderNumber").val(),
      comCustomerId: parseInt($("#customerId").val(), 10),
      orderDate: $("#orderDate").val(),
      address: $("#address").val(),
    };

    // Collect item data from DataTable
    const salesOrderItems = itemTable
      .rows()
      .data()
      .toArray()
      .map((row) => ({
        itemName: row.itemName,
        quantity: row.quantity,
        price: row.price,
        total: row.total,
      }));

    // Combine header and items into one object
    const salesOrderData = {
      Order: salesOrderHeader,
      OrderItem: salesOrderItems,
    };

    console.log(salesOrderData);
    console.log(typeof salesOrderHeader.comCustomerId);
    // Send data to the server
    $.ajax({
      url: "/SalesOrder/Save",
      type: "POST",
      // contentType: "application/json",
      data: salesOrderData,
      success: function (response) {
        swal.fire({
          title: "Success",
          text: "Sales Order created successfully!",
          icon: "success",
        });
        //delay 2 seconds before redirecting
        setTimeout(function () {
          window.location.href = "/SalesOrder"; // Redirect to the sales order list
        }, 2000);
      },
      error: function (xhr, status, error) {
        console.error("Error creating sales order:", error);
      },
    });
  });
});

function formatCurrency(value) {
  return new Intl.NumberFormat("id-ID", {
    style: "currency",
    currency: "IDR",
    minimumFractionDigits: 0,
  }).format(value);
}

function populateCustomerDropdown(data) {
  const customerSelect = $("#customerId");
  customerSelect.empty(); // Clear existing options
  customerSelect.append(
    '<option value="" disabled selected>Select a customer</option>'
  );
  data.forEach(function (customer) {
    customerSelect.append(
      `<option value="${customer.comCustomerId}">${customer.customerName}</option>`
    );
  });
}
