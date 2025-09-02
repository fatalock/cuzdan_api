using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuzdan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_user_balance_by_currency(p_user_id UUID)
                RETURNS TABLE(currency_type INT, total_balance NUMERIC) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT
                        ""Currency"",
                        SUM(""Balance"")
                    FROM
                        ""Wallets""
                    WHERE
                        ""UserId"" = p_user_id
                    GROUP BY
                        ""Currency"";
                END;
                $$ LANGUAGE plpgsql;
            ");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE PROCEDURE transfer_funds(
                p_sender_wallet_id UUID,
                p_receiver_wallet_id UUID,
                p_amount NUMERIC,
                p_conversion_rate NUMERIC
            )
            LANGUAGE plpgsql
            AS $$
            DECLARE
                sender_wallet_record RECORD;
                receiver_wallet_record RECORD;
                converted_amount NUMERIC;
            BEGIN
                SELECT * INTO sender_wallet_record FROM ""Wallets"" WHERE ""Id"" = p_sender_wallet_id FOR UPDATE;
            SELECT* INTO receiver_wallet_record FROM ""Wallets"" WHERE ""Id"" = p_receiver_wallet_id FOR UPDATE;

                converted_amount:= p_amount * p_conversion_rate;

            UPDATE ""Wallets""
                SET ""AvailableBalance"" = ""AvailableBalance"" - p_amount,
                    ""Balance"" = ""Balance"" - p_amount
                WHERE ""Id"" = p_sender_wallet_id;

            UPDATE ""Wallets""
                SET ""AvailableBalance"" = ""AvailableBalance"" + converted_amount,
                    ""Balance"" = ""Balance"" + converted_amount
                WHERE ""Id"" = p_receiver_wallet_id;

            INSERT INTO ""Transactions""(""Id"", ""FromId"", ""ToId"", ""OriginalAmount"", ""OriginalCurrency"", ""ConvertedAmount"", ""TargetCurrency"", ""ConversionRate"", ""Status"", ""Type"", ""CreatedAt"")
                VALUES(
                    gen_random_uuid(),
                    p_sender_wallet_id,
                    p_receiver_wallet_id,
                    p_amount,
                    sender_wallet_record.""Currency"",
                    converted_amount,
                    receiver_wallet_record.""Currency"",
                    p_conversion_rate,
                    'completed'::public.transaction_status,
                    'transfer'::public.transaction_type,
                    NOW()
                );

            END;
            $$;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS get_user_balance_by_currency(UUID);
            ");

            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS transfer_funds(
                p_sender_wallet_id UUID,
                p_receiver_wallet_id UUID,
                p_amount NUMERIC,
                p_conversion_rate NUMERIC
            );
            ");
        }
    }
}
