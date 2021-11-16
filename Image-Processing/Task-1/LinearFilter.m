
function [new] = LinearFilter(I,Filter,Postproc)

new = I;
% padding

I = padarray(I, [1, 1]);
[x,y] = size(I); 
for i = 2:x-1
    for j = 2:y-1
        %for k = 1:z
            sum = 0;
            for ii = 1:3  
                for jj = 1:3
                    sum = sum + I(i + ii - 2,j + jj - 2) * Filter(ii, jj);
                end
            end
            %Postproc
            if(Postproc == "none")
                new(i - 1,j - 1) = sum;
            elseif(Postproc == "cutoff")
                if(sum < 0)
                    new(i - 1,j - 1) = 0;
                elseif(sum > 255)
                    new(i - 1,j - 1) = 255;
                else
                    new(i - 1,j - 1) = int8(sum);
                end  
            elseif(Postproc == "absolute")
             sum=abs(sum);
             if(sum > 255)
                    new(i - 1,j - 1) = int8(255);
                else
                    new(i - 1,j - 1) = int8(sum);
             end 
            end
        % end
    end
end


end
