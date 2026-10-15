using activity_00_tap_26_27.Core;
using GameLibrary.Csv;
using GameLibrary.CSV;
using System;
using System.Collections.Generic;

namespace GameLibrary.Replay
{
    public class ReplayReader
    {
        public static ReplayData Read(CsvTable csv_table, ContentReport error_report)
        {
            ReplayData replay_data = null;
            int end_fixed_step_index = -1;
            int seed = -1;
            List<ReplayActionData> replay_action_table = new List<ReplayActionData>();

            if (csv_table.GetColumnIndex("Step") < 0)
            {
                error_report.AddError(csv_table.GetFileName(), 0, $"missing Step columns");
            }

            if (csv_table.GetColumnIndex("Type") < 0)
            {
                error_report.AddError(csv_table.GetFileName(), 0, $"missing Type columns");
            }

            if (csv_table.GetColumnIndex("Value") < 0)
            {
                error_report.AddError(csv_table.GetFileName(), 0, $"missing Value columns");
            }

            if (error_report.GetErrorCount() <= 0)
            {
                for (int current_line_index = 0; current_line_index < csv_table.GetRowCount(); current_line_index++)
                {
                    int human_readable_line_index = current_line_index + 2;

                    CsvRow current_row = csv_table.GetRowAtIndex(current_line_index);

                    string fixed_step_index_read_text = current_row.GetCellAtIndex(0);

                    if (!int.TryParse(fixed_step_index_read_text, out int fixed_step_index))
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"Invalid fixed frame index: {fixed_step_index_read_text}, should be an integer");
                    }
                    else if (fixed_step_index < 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"Invalid fixed frame index: {fixed_step_index_read_text}, should be > 0");
                    }

                    string replay_element_type = current_row.GetCellAtIndex(1);

                    if (current_line_index == 0)
                    {
                        if (replay_element_type == "SEED")
                        {
                            string seed_read_text = current_row.GetCellAtIndex(2);

                            if (!int.TryParse(seed_read_text, out seed))
                            {
                                error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"Invalid seed: {seed_read_text}, should be an integer");
                            }
                        }
                        else
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"No SEED found");
                        }
                    }
                    else
                    {
                        if (replay_element_type == "END")
                        {
                            if (current_line_index != csv_table.GetRowCount() - 1)
                            {
                                error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"END has to be the last line of the file");
                            }

                            end_fixed_step_index = fixed_step_index;
                        }
                        else if (replay_element_type == "ACTION")
                        {
                            string action_read_text = current_row.GetCellAtIndex(2);

                            if (!Enum.IsDefined(typeof(GameActionType), action_read_text))
                            {
                                error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"Invalid GameActionType: \"{action_read_text}\", should be an GameActionType");
                            }
                            else
                            {
                                if ((replay_action_table.Count > 0) && (replay_action_table[replay_action_table.Count - 1]._fixedStepIndex > fixed_step_index))
                                {
                                    error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"fixed frame index {fixed_step_index} should not be lower that the previous");
                                }

                                replay_action_table.Add(new ReplayActionData(fixed_step_index, Enum.Parse<GameActionType>(current_row.GetCellAtIndex(2))));
                            }
                        }
                        else
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_line_index, $"Invalid Replay element: {replay_element_type}");
                        }
                    }
                }
            }

            if (seed == -1)
            {
                error_report.AddError(csv_table.GetFileName(), -1, $"No SEED found");
            }

            if (error_report.GetErrorCount() <= 0)
            {
                replay_data = new ReplayData(seed, replay_action_table, end_fixed_step_index);
            }

            return replay_data;
        }
    }
}