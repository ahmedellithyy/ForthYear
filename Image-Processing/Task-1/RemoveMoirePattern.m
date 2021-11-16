I = imread('Moire1.bmp'); 
figure,imshow(I,[]),title('Original Image'); 
I = rgb2gray(I); 

FFTImage = fft2(I); 
shiftedImage=fftshift(FFTImage); 
figure,imshow(shiftedImage,[]),title('Shifted Image'); 

loggedImage = log(abs(shiftedImage)); 
figure,imshow(loggedImage,[]),title('Image in Log Scale'); 
 
H = ones(size(shiftedImage));

x = 10;

% Moire2.bmp 
% H(165-x:169+x,127-x:131+x)=0; 
% H(170-x:174+x,61-x:65+x)=0; 
 
%Moire1.bmp
 H(74-x:90+x,262-x:275+x)=0; 
H(250-x:265+x,110-x:140+x)=0; 
 H(212-x:227+x,240-x:250+x)=0; 
 H(120-x:153+x,260-x:262+x)=0; 
 
 
FilteredImage = shiftedImage.*H; 
 
F = ifftshift(FilteredImage); 
FinalImage = ifft2(F); 
 
figure,imshow(FinalImage,[]),title('Image Without Noise');
