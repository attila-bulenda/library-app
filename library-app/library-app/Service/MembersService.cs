using AutoMapper;
using library_app.Models.MemberDtos;
using library_app.Contracts;

namespace library_app.Service
{
    public class MembersService
    {
        private readonly IMembersRepository _membersRepository;
        private readonly IMapper _mapper;
        public MembersService(IMembersRepository membersRepository,IMapper mapper)
        {
            _membersRepository = membersRepository;
            _mapper = mapper;   
        }

        public async Task<MemberFullDto> GetMemberAsync(int id)
        {
            var result = await _membersRepository.FindMemberWithBooksLoanedAsync(id);
            if (result == null)
            {
                return null;
            }
            var member = _mapper.Map<MemberFullDto>(result);
            return member;
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var result = await _membersRepository.GetAllAsync();
            return _mapper.Map<List<MemberDto>>(result);
        }
    }
}
