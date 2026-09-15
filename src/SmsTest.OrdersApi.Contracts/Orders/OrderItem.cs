using System.Diagnostics.CodeAnalysis;

namespace SmsTest.OrdersApi.Contracts.Orders;

public sealed class OrderItem
{
    private string _id;

    private double _quantity;

    public required string Id
    {
        get => this._id;

        [MemberNotNull(member: nameof(_id))]
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            this._id = value;
        }
    }

    public required double Quantity
    {
        get => this._quantity;

        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(this.Quantity),
                    value,
                    message: "Quantity must be greater than or equal to zero."
                );
            }

            this._quantity = value;
        }
    }
}
