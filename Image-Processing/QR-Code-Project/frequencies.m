function [ freq ] = frequencies(row)
    %number of elements in the row
    numOf_col = numel(row);
    freq = [];
    
% [center counter color]
    ccc = [1 0 row(1)];
    for j = 1:numOf_col
        if ccc(3) == row(j)
            ccc(2) = ccc(2) + 1;
        else
            ccc(1) = ccc(1) + uint32(ccc(2))/2;
            freq = [freq ; ccc];
            ccc = [j 1 row(j)];
        end
    end
    ccc(1) = ccc(1)+ uint32(ccc(2))/2;
    freq = [freq ; ccc];
end


