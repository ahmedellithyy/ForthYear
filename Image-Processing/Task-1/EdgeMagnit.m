function [m] = EdgeMagnit(I)

x = LinearFilter(I, Sobel("H"), "absolute");
y = LinearFilter(I, Sobel("V"), "absolute");
m = x;
[a, b, c] = size(x);
for i = 1:a
   for j = 1:b
      for k = 1: c
         if(x(i, j, k) + y(i, j, k) > 255)
             m(i, j, k) = 255;
         else 
             m(i, j, k) = x(i, j, k) + y(i, j, k);
         end
      end
       
   end
    
end
end